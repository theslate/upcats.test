using System.Collections.Generic;

namespace UpCataas.Models
{
    public class User
    {
        /// <summary>
        /// User id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// List of cached image keys that user has generated.
        /// </summary>
        public IEnumerable<string> ImageHistory { get; set; }
    }
}
