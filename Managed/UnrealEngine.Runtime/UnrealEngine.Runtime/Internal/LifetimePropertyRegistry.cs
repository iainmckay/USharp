using System.Collections.Generic;

namespace UnrealEngine.Runtime
{
    public sealed class LifetimePropertyRegistry
    {
        private UObject obj;

        private List<FLifetimeProperty> dest;

        public LifetimePropertyRegistry(UObject obj, List<FLifetimeProperty> dest)
        {
            this.obj = obj;
            this.dest = dest;
        }

        public void Add(string propertyName)
        {
            UProperty property = FindProperty(propertyName);

            for (ushort i = 0; i < property.ArrayDim; i++)
            {
                dest.Add(new FLifetimeProperty((ushort) (property.RepIndex + i)));
            }
        }
 
        private UProperty FindProperty(string propertyName)
        {
            UClass unrealClass = obj.GetClass();
            UProperty property = unrealClass.FindPropertyByName(new FName(propertyName));

            if (! (FBuild.BuildShipping || FBuild.BuildTest))
            {
                if (null == property)
                {
                    FMessage.Log(FMessage.LogNet, ELogVerbosity.Fatal, $"Attempt to replicate property '{propertyName}' which does not exist.");
                }
                else if (!property.HasAnyPropertyFlags(EPropertyFlags.Net))
                {
                    FMessage.Log(FMessage.LogNet, ELogVerbosity.Fatal, $"Attempt to replicate property '{propertyName}' that was not tagged to replicate! Please use 'Replicated' or 'ReplicatedUsing' keyword in the UProperty() declaration.");
                }
            }

            return property;
        }
    }
}
