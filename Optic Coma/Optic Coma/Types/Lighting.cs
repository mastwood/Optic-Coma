using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Optic_Coma
{
    // Considering using an interface for lighting
    // What this does is allow you to call a new instance of "Lighting
    // Then implement the methods and variables you need without having to go through the lines
    // It also lets you change pretty much anything you need
    // Since classes can only inherit from one other class at a time, this allows you to implement multiple features
    // Between classes without having to rely on polymorphism and inheritance
    // Naming convention for interfaces: interface IName<T>
    public interface ILighting<T>
    {
        // This is the "signature" not sure what its uses is yet... researching
        bool Equals(T obj);
    }
}
