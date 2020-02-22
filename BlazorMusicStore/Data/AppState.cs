using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMusicStore.Data
{
    public class AppState
    {
        public int CartCount { get; set; }
        private string _shoppingCardId { get; set; }

        public event Action OnChange;

        public void IncrementCartCount()
        {
            CartCount++;
            NotifyStateChanged();
        }

        public void DecrementCartCount()
        {
            CartCount--;
            NotifyStateChanged();
        }

        public void ZeroOutCartCount()
        {
            CartCount = 0;
            NotifyStateChanged();
        }


        private void NotifyStateChanged() => OnChange?.Invoke();
        
        public string ShippingCartId
        {
            get
            {
                if (String.IsNullOrEmpty(_shoppingCardId))
                {
                    _shoppingCardId = Guid.NewGuid().ToString();
                }
                return _shoppingCardId;
            }
            set
            {
                _shoppingCardId = value;
            }
        }

        public int OrderNumber { get; set; }
    }
}
