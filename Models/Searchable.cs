using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JazzNotes.Models
{
    public class Searchable
    {
        /// <summary>
        /// Name of the searchable object.
        /// </summary>
        public string Name { get; protected set; }
    }
}
