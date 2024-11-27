using System;

namespace LHQ.Generators
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TemplateGeneratorIdAttribute : Attribute
    {
        public string Id { get; set; }

        public TemplateGeneratorIdAttribute(string id)
        {
            Id = id;
        }
    }
}