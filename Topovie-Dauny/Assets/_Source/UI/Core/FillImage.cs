using UnityEngine;

namespace UI.Core
{
    public class FillImage : MonoBehaviour
    {
        public float FillAmount{ get; private set; }
        
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            var sprite = _spriteRenderer.sprite;
            
            if (!sprite.texture.isReadable)
            {
                Debug.LogError("The texture is not readable. Please check the import settings.");
                return;
            }

            
            var textureWidth = sprite.texture.width;
            var textureHeight = sprite.texture.height;

            var texture = new Texture2D(textureWidth, textureHeight);
            var pixels = _spriteRenderer.sprite.texture.GetPixels();

            for (var y = 0; y < textureHeight; y++)
            {
                for (var x = 0; x < textureWidth; x++)
                {
                    if (y < textureHeight * FillAmount)
                    {
                        texture.SetPixel(x, y, pixels[y * textureWidth + x]);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }

            texture.Apply();

            var newSprite = Sprite.Create(texture, new Rect(0, 0, textureWidth, textureHeight),
                new Vector2(0.5f, 0.5f));
            _spriteRenderer.sprite = newSprite;
        }
    }
}