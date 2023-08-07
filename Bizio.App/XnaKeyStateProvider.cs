using Hyjynx.Core.Services;
using System;
using System.Collections.Generic;

namespace Hyjynx.App.Xna
{
    public class XnaKeyStateProvider : IKeyStateProvider
    {
        public XnaKeyStateProvider()
        {
            var map = new Dictionary<Microsoft.Xna.Framework.Input.Keys, Keys>();
            var xnaKeys = Enum.GetValues<Microsoft.Xna.Framework.Input.Keys>();
            foreach (var xnaKey in xnaKeys)
            {
                if (Enum.TryParse<Keys>($"{xnaKey}", out var key))
                {
                    map[xnaKey] = key;
                }
            }
            _keyMap = map;
        }

        public IReadOnlyDictionary<Keys, KeyState> GetKeyStates()
        {
            var keyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            var output = new Dictionary<Keys, KeyState>();

            foreach ((var xnaKey, var key) in _keyMap)
            {
                try
                {
                    output.Add(key, (KeyState)keyState[xnaKey]);
                }
                catch { }
            }

            return output;
        }

        private readonly IReadOnlyDictionary<Microsoft.Xna.Framework.Input.Keys, Keys> _keyMap;
    }
}
