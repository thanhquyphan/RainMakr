using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMakr.Web.Unity
{
    using System.Diagnostics.Contracts;
    using System.Web.Http.Dependencies;

    using Microsoft.Practices.Unity;

    /// <summary>
    /// The <see cref="UnityResolutionScope"/>
    ///   class is used to provide type resolution support using an <see cref="IUnityContainer"/> instance.
    /// </summary>
    public class UnityResolutionScope : IDependencyScope
    {
        /// <summary>
        /// Stores the container used to resolve instances.
        /// </summary>
        private readonly IUnityContainer _container;

        /// <summary>
        /// Stores whether this instance has been disposed.
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Stores the resolved instance.
        /// </summary>
        private object _resolvedInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityResolutionScope"/> class.
        /// </summary>
        /// <param name="container">
        /// The container. 
        /// </param>
        public UnityResolutionScope(IUnityContainer container)
        {
            Contract.Requires<ArgumentNullException>(container != null);

            _container = container;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">
        /// Type of the service. 
        /// </param>
        /// <returns>
        /// A <see cref="Object"/> instance. 
        /// </returns>
        public object GetService(Type serviceType)
        {
            if (_resolvedInstance != null)
            {
                throw new InvalidOperationException("This scope has already resolved an instance.");
            }

            if (_container.IsRegistered(serviceType) == false)
            {
                return null;
            }

            _resolvedInstance = _container.Resolve(serviceType);

            return _resolvedInstance;
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <param name="serviceType">
        /// Type of the service. 
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerable&lt;T&gt;"/> instance. 
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (_resolvedInstance != null)
            {
                throw new InvalidOperationException("This scope has already resolved an instance.");
            }

            if (_container.IsRegistered(serviceType) == false)
            {
                return new List<object>();
            }

            _resolvedInstance = _container.ResolveAll(serviceType);

            return (IEnumerable<object>)_resolvedInstance;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources. 
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                // Free managed resources
                if (_resolvedInstance != null)
                {
                    _container.Teardown(_resolvedInstance);
                    _resolvedInstance = null;
                }
            }

            // Free native resources if there are any
            _isDisposed = true;
        }
    }
}