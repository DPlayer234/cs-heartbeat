using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Heartbeat
{
    /// <summary>
    ///     Defines the configuration of the <see cref="EngineInstance"/>.
    /// </summary>
    public class EngineConfig
    {
        /// <summary> Creates a new <seealso cref="EngineConfig"/>. </summary>
        public EngineConfig() { }
        
        /// <summary> The <seealso cref="ContentManager.RootDirectory"/> of the Engine's <seealso cref="ContentManager"/>. </summary>
        public string ContentRoot = "Content";

        /// <summary>
        ///     The default <seealso cref="EngineConfig"/> when none is
        ///     passed to the <seealso cref="EngineInstance.Create(EngineConfig)"/>.
        ///     May be modified.
        /// </summary>
        public static EngineConfig Default = new EngineConfig();
    }
}
