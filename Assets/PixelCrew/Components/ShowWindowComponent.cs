using PixelCrew.Utils;
using UnityEngine;

namespace PixelCrew.Components
{
    public class ShowWindowComponent : MonoBehaviour
    {
        [SerializeField] private string[] _path;

        public void Show(int id)
        {
            WindowUtils.CreateWindow(_path[id]);
        }
    }
}