using System.IO;
using System.Threading.Tasks;

using Android.Content.Res;
using Android.Graphics;

using Demo.Droid;

using HB.FullStack.Mobile.Platforms;

using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace Demo.Droid
{
    /// <summary>
    /// 不能放在HB.Framework.Client.Droid中，因为要用到项目中的Resources类
    /// </summary>
    public class FileHelper : IFileHelper
    {
        //TODO: 确定各个图像的位置和大小
        public static readonly string AvatarDirectory = System.IO.Path.Combine(FileSystem.AppDataDirectory, "avatars");
        public static readonly int Avatar_Max_Height = 1024;
        public static readonly int Avatar_Max_Width = 1024;

        private readonly object _locker = new object();

        public async Task SaveAvatarAsync(ImageSource imageSource, long userId)
        {
            if (!Directory.Exists(AvatarDirectory))
            {
                lock (_locker)
                {
                    if (!Directory.Exists(AvatarDirectory))
                    {
                        Directory.CreateDirectory(AvatarDirectory);
                    }
                }
            }

            using Bitmap bitmap = await imageSource.GetBitMapAsync().ConfigureAwait(false);

            using Bitmap scaledBitmap = bitmap.ScaleTo(Avatar_Max_Height, Avatar_Max_Width);

            string path = GetAvatarFilePath(userId);

            using FileStream fileStream = new FileStream(path, FileMode.Create);

            bool result = await scaledBitmap.CompressAsync(Bitmap.CompressFormat.Png, 100, fileStream).ConfigureAwait(false);

            await fileStream.FlushAsync().ConfigureAwait(false);
        }

        public string? GetAvatarFilePath(long userId)
        {
            string path = System.IO.Path.Combine(AvatarDirectory, $"{userId}.png");

            return File.Exists(path) ? path : null;
        }

        public async Task<byte[]?> GetAvatarAsync(long userId)
        {
            string? filePath = GetAvatarFilePath(userId);

            if (filePath == null)
            {
                return null;
            }

            using FileStream fileStream = new FileStream(filePath, FileMode.Open);

            using MemoryStream memoryStream = new MemoryStream();

            await fileStream.CopyToAsync(memoryStream).ConfigureAwait(false);

            return memoryStream.ToArray();
        }

        public async Task<Stream> GetResourceStreamAsync(string resourceName)
        {
            int resId = GetResourceId(resourceName);

            using Stream stream = Platform.CurrentActivity.Resources.OpenRawResource(resId);

            MemoryStream memoryStream = new MemoryStream();

            await stream.CopyToAsync(memoryStream).ConfigureAwait(false);

            memoryStream.Position = 0;

            return memoryStream;
        }

        public static int GetResourceId(string resourceName)
        {
            string withoutExtensionFileName = System.IO.Path.GetFileNameWithoutExtension(resourceName);

            //int resId = Platform.CurrentActivity.Resources.GetIdentifier(withoutExtensionFileName, "drawable", "com.brlite.mycolorfultime");
            int resId = (int)typeof(Resource.Drawable).GetField(withoutExtensionFileName).GetValue(null);
            return resId;
        }

        public string GetAssetsHtml(string name)
        {
            AssetManager assetsManager = Platform.CurrentActivity.Assets;

            using Stream stream = assetsManager.Open(name);

            using StreamReader reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}