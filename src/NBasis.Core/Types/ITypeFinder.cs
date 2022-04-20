using System;
using System.Collections.Generic;

namespace NBasis.Types
{
    public interface ITypeFinder
    {
        /// <summary>
        /// Finds types assignable from of a certain type in the finder
        /// </summary>
        IEnumerable<Type> GetDerivedTypes<TBase>() where TBase : class;

        /// <summary>
        /// Get all of the implementations of a given interface
        /// </summary>
        IEnumerable<Type> GetInterfaceImplementations<TInterface>() where TInterface : class;
    }
}
