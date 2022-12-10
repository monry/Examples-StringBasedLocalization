using UnityEngine;
using UnityEngine.Localization;

namespace Monry.StringBasedLocalization
{
    public class SampleView : MonoBehaviour
    {
        [SerializeField] private LocalizedString _localizedString;

        public void Start()
        {
            Debug.Log(_localizedString.GetLocalizedString());
        }
    }
}
