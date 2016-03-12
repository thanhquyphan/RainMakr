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
    /// The <see cref="UnityDependencyResolver"/>
    ///   class is used to provide dependency resolution support using an <see cref="IUnityContainer"/> instance.
    /// </summary>
    public class UnityDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// Stores whether this instance has been disposed.
        /// </summary>
        private bool _isDisposed;

        private IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityDependencyResolver"/> class.
        /// </summary>
        /// <param name="container">
        /// The container. 
        /// </param>
        public UnityDependencyResolver(IUnityContainer container)
        {
            Contract.Requires<ArgumentNullException>(container != null);

            _container = container;
        }

        /// <summary>
        /// Begins the scope.
        /// </summary>
        /// <returns>
        /// A <see cref="IDependencyScope"/> instance. 
        /// </returns>
        public IDependencyScope BeginScope()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            return new UnityResolutionScope(_container);
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
            return this._container.Resolve(serviceType);
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
            return this._container.ResolveAll(serviceType);
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
                _container.Dispose();
            }

            // Free native resources if there are any
            _isDisposed = true;
        }
    }
}