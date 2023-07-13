using System;
using Base;

namespace Other
{
    public class PresenterFactory
    {
        public static TPresenter Create<TModel, TPresenter, TView>(TView view)
            where TModel : BaseModel, new()
            where TView : BaseView
            where TPresenter : BasePresenter<TModel, TView>
        {
            TModel model = new TModel();
            TPresenter presenter = (TPresenter)Activator.CreateInstance(typeof(TPresenter), model, view);
        
            return presenter;
        }
    }
}