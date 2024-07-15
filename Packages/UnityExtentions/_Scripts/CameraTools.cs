using UnityEngine;

namespace UnityExtentions
{
    public static class CameraTools
    {
        /// <summary>
        /// Creates a sprite using the target texture's width and height.
        /// </summary>
        /// <param name="targetCamera"></param>
        /// <returns></returns>
        public static Sprite CaptureScreen(Camera targetCamera)
        {
            return CaptureScreen(targetCamera, targetCamera.targetTexture.width, targetCamera.targetTexture.height);
        }

        /// <summary>
        /// Creates a sprite from the camera.
        /// </summary>
        /// <param name="targetCamera">The camera. The target texture must be set.</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>Sprite from the camera. If target texture is null, returns null.</returns>
        public static Sprite CaptureScreen(Camera targetCamera, int width, int height)
        {
            if (targetCamera.targetTexture == null) return null;

            Rect rect = new Rect(0, 0, width, height);
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            targetCamera.Render();

            RenderTexture currentRenderTexture = RenderTexture.active;
            RenderTexture.active = targetCamera.targetTexture;
            texture.ReadPixels(rect, 0, 0);
            texture.Apply();

            RenderTexture.active = currentRenderTexture;

            Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);

            return sprite;
        }

        /// <summary>
        /// Creates a sprite from the camera.
        /// </summary>
        /// <param name="targetCamera">The camera.</param>
        /// <param name="renderTexture">The render texture will be temporarily applied to the camera.</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>Sprite from the camera.</returns>
        public static Sprite CaptureScreen(Camera targetCamera, RenderTexture renderTexture, int width, int height)
        {
            Rect rect = new Rect(0, 0, width, height);
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            targetCamera.targetTexture = renderTexture;
            targetCamera.Render();

            RenderTexture currentRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            texture.ReadPixels(rect, 0, 0);
            texture.Apply();

            targetCamera.targetTexture = null;
            RenderTexture.active = currentRenderTexture;

            Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);

            return sprite;
        }
    }
}