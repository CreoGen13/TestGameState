using System;
using UniRx;

namespace Base
{
    public abstract class BasePresenter<TModel, TView>
        where TModel : BaseModel
        where TView : BaseView
    {
        protected TModel Model;
        protected TView View;
        
        protected readonly CompositeDisposable Disposable = new CompositeDisposable();

        protected abstract void Init();

        protected BasePresenter(TModel model, TView view)
        {
            View = view;
            Model = model;
        }

        public void Dispose()
        {
            Disposable.Dispose();
        }
    }
}