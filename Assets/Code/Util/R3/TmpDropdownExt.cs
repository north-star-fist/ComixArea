using System.Threading;
using R3;
using TMPro;

namespace ComixArea.Util
{
    public static class TmpDropdownExt
    {
        #region R3-TMP compatible extensions
        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static Observable<int> OnValueChangedAsObservable(this TMP_Dropdown dropdown)
        {
            return Observable.Create<int, TMP_Dropdown>(dropdown, static (observer, d) =>
            {
                observer.OnNext(d.value);
                return d.onValueChanged.AsObservable(GetDestroyCancellationToken(d)).Subscribe(observer);
            });
        }

        private static CancellationToken GetDestroyCancellationToken(TMP_Dropdown value)
        {
            // UNITY_2022_2_OR_NEWER has MonoBehavior.destroyCancellationToken
#if UNITY_2022_2_OR_NEWER
            return value.destroyCancellationToken;
#else
            var component = value.gameObject.GetComponent<R3.Triggers.ObservableDestroyTrigger>();
            if (component == null)
            {
                component = value.gameObject.AddComponent<R3.Triggers.ObservableDestroyTrigger>();
            }
            return component.GetCancellationToken();
#endif
        }
        #endregion
    }
}
