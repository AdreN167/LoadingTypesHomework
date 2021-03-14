using System;
using System.Collections.Generic;

namespace LoadingTypesHomework.Models
{
    public class Directory : Entity
    {
        public string Path { get; set; }
        public string Name { get; set; }

        public virtual ICollection<File> Files { get; set; }
        public virtual ICollection<Directory> Directories { get; set; }
    }
}
