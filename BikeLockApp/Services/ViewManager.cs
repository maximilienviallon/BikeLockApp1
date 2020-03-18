using System;
using System.Globalization;
using System.Reflection;
using Xamarin.Forms;

namespace BikeLockApp.Services
{
    public class ViewManager
    {
        // Reference to the content control 
        public static ContentPage ContentControlRef { get; set; } = null;

        /// <summary>
        /// This method is called whenever we need to change the view of the User Control in the MainView
        /// </summary>
        /// <param name="viewModelType"></param>
        public static ContentPage ChangeView(Type viewModelType)
        {
            // Creates the User Control based on the ViewModel, it will get the correct view through reflection and naming conventions in the MVVM pattern
            // This means, that the viewModel type/name must have the same name as the View + the word "Model". Otherwise this wont work
            ContentPage tmpUC = CreatePage(viewModelType, null);

            // We are using Types, so that we don't have to instansiate all ViewModel objects before we need them. This is done in cooperation with the Lazy class in the ServiceContainer class. 
            var viewModel = ServiceContainer.Resolve(viewModelType);


            // If viewModel is not null we will set the new data context, so that we have access to the methods, and properties in the ViewModel that belongs to the current view
            // After that we update the content of the ContentControl. This one is refering (or should be refering) to the actual ContentControl in the MainView. The reference is made in the MainViewModel to begin with. 
            if (viewModel != null)
            {
                tmpUC.BindingContext = viewModel;
                ViewManager.ContentControlRef = tmpUC;
                return ViewManager.ContentControlRef;
            }

            return null;

        }


        /// <summary>
        /// Based on the ViewModel type we use this method to remove the word "Model" in order to get the correct View
        /// Therefore it is important that don't make any spelling errors in the namings in order for this to work. 
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        private static Type GetPageTypeForViewModel(Type viewModelType)
        {
            var viewName = viewModelType.FullName.Replace("Model", string.Empty);
            var viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;
            var viewAssemblyName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
            var viewType = Type.GetType(viewAssemblyName);

            Console.WriteLine("Called GetPageTypeForViewModel. Type is now: " + viewType);

            return viewType;
        }

        /// <summary>
        /// This method creates the View itself and returns it to the caller. 
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static ContentPage CreatePage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);
            if (pageType == null)
            {
                throw new Exception($"Cannot locate page type for {viewModelType}");
            }
            ContentPage page = Activator.CreateInstance(pageType) as ContentPage;

            return page;
        }
    }
}
