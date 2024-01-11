using UnityEngine;

namespace Utils
{
    public class ScreenScaler : MonoBehaviour
    {
        private void Start()
        {
            var sprite = GetComponent<SpriteRenderer>().sprite;

            float width = sprite.bounds.size.x;
            float height = sprite.bounds.size.y;

            var mainCamera = Camera.main;
            if (mainCamera == null)
                throw new RapWayException("No camera!");

            float worldScreenHeight = mainCamera.orthographicSize * 2.0f;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            transform.localScale = new Vector2(worldScreenWidth / width, worldScreenHeight / height);
        }
    }
}
