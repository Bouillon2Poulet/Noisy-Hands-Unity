using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mediapipe.Tasks.Vision.HandLandmarker;

namespace Mediapipe.Unity.Tutorial
{
    public class HandLandMarks : MonoBehaviour
    {
        [SerializeField] private TextAsset _configAsset;
        [SerializeField] private RawImage _screen;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private int _fps;

        private WebCamTexture _webCamTexture;
        private CalculatorGraph _graph;
        private Texture2D _inputTexture;
        private Color32[] _pixelData;

        private IEnumerator Start()
        {
            Protobuf.SetLogHandler(Protobuf.DefaultLogHandler);

            //Webcam
            if (WebCamTexture.devices.Length == 0)
            {
                throw new System.Exception("Web Camera devices are not found");
            }
            var webCamDevice = WebCamTexture.devices[0];
            _webCamTexture = new WebCamTexture(webCamDevice.name, _width, _height, _fps);
            _webCamTexture.Play();

            yield return new WaitUntil(() => _webCamTexture.width > 16);

            _screen.rectTransform.sizeDelta = new Vector2(_width, _height);
            _screen.texture = _webCamTexture;

            //Load model file
            // yield return AssetLoader.PrepareAssetAsync(config.ModelPath);

            //Mediapipe
            _graph = new CalculatorGraph(_configAsset.text);
            // _graph.AddPacketToInputStream("NUM_HANDS:num_hands", Packet.CreateInt(1));
            // _graph.AddPacketToInputStream("MODEL_COMPLEXITY:model_complexity", Packet.CreateInt(1));
            // _graph.AddPacketToInputStream("USE_PREV_LANDMARKS:use_prev_landmarks", Packet.CreateBool(true));

            _graph.StartRun();

            _inputTexture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
            _pixelData = new Color32[_width * _height];

            while (true) //On passe la texture de la caméra au graph mediapipe
            {
                _inputTexture.SetPixels32(_webCamTexture.GetPixels32(_pixelData));
                var imageFrame = new ImageFrame(ImageFormat.Types.Format.Srgba, _width, _height, _width * 4, _inputTexture.GetRawTextureData<byte>());
                _graph.AddPacketToInputStream("IMAGE:image", Packet.CreateImageFrame(imageFrame));

                yield return new WaitForEndOfFrame();
            }
        }

        private void OnDestroy()
        {
            if (_webCamTexture != null)
            {
                _webCamTexture.Stop();
            }

            if (_graph != null)
            {
                try
                {
                    _graph.CloseInputStream("input_video");
                    _graph.WaitUntilDone();
                }
                finally
                {
                    _graph.Dispose();
                    _graph = null;
                }
            }
        }
    }
}