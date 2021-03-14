using System;

namespace LoadingTypesHomework.Models
{
    public class File : Entity
    {
        public string Path { get; set; }
        public string Name { get; set; }
        
        public Guid ParentDirectoryId { get; set; }
        public virtual Directory ParentDirectory { get; set; }
    }
}
