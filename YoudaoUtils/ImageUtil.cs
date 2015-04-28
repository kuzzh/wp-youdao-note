
namespace YoudaoNoteUtils
{
    using System;
    using System.IO;
    using System.Windows;

    public enum ImageType
    {
        Jpg,
        Bmp,
        Png,
        Gif,
        Unknown
    }
    public static class ImageUtil
    {
        public static Size GetImageSize(ImageType imgType, byte[] buffer)
        {
            var size = new Size(0, 0);
            switch (imgType)
            {
                case ImageType.Jpg:
                    size = GetJpegSize(buffer);
                    break;
                case ImageType.Bmp:
                    size = GetBmpSize(buffer);
                    break;
                case ImageType.Png:
                    size = GetPngSize(buffer);
                    break;
                case ImageType.Gif:
                    size = GetGifSize(buffer);
                    break;
                case ImageType.Unknown:
                    break;
            }

            return size;
        }

        public static ImageType GetImageType(byte[] buffer)
        {
            var pngType = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            var fileHead = new byte[8];

            Array.Copy(buffer, fileHead, 8);
            var imgeType = ImageType.Unknown;
            switch (fileHead[0])
            {
                case 0xff:
                    if (fileHead[1] == 0xd8)
                    {
                        imgeType = ImageType.Jpg;//jpeg
                    }
                    break;
                case 0x47:
                    if (fileHead[1] == 0x49 && fileHead[2] == 0x46 && fileHead[3] == 0x38 && (fileHead[4] == 0x39 || fileHead[4] == 0x37)
                        && fileHead[5] == 0x61)
                    {
                        imgeType = ImageType.Gif; // gif
                    }
                    break;
                case 0x42:
                    if (fileHead[1] == 0x4D)
                    {
                        imgeType = ImageType.Bmp;//bmp
                    }
                    break;
                case 0x89:
                    if (fileHead[1] == pngType[1] && fileHead[2] == pngType[2] && fileHead[3] == pngType[3] && fileHead[4] == pngType[4] &&
                        fileHead[5] == pngType[5] && fileHead[6] == pngType[6] && fileHead[7] == pngType[7])
                    {
                        imgeType = ImageType.Png;//png
                    }
                    break;
                default:
                    imgeType = ImageType.Unknown;
                    break;
            }
            return imgeType;
        }

        private static Size GetBmpSize(byte[] buffer)
        {
            //Bmp 图片前2字节：0x42 4D
            var imgType = GetImageType(buffer);
            if (imgType != ImageType.Bmp)
            {
                throw new YoudaoNoteException("not bmp file");
            }

            //bmp图片的宽度信息保存在第 18-21位 4字节
            //bmp图片的高度度信息保存在第 22-25位 4字节
            //读取宽度，高度 各4字节
            var size = new Size {Width = BitConverter.ToInt32(buffer, 18), Height = BitConverter.ToInt32(buffer, 22)};


            return size;
        }

        private static Size GetGifSize(byte[] buffer)
        {
            //gif图片信息域(47 49 46 38 39|37 61) GIF89(7)a，共6字节
            //根据6字节判断是否为gif图片
            var imgType = GetImageType(buffer);
            if (imgType != ImageType.Gif)
            {
                throw new YoudaoNoteException("not gif file");
            }

            //读取宽度，高度 各2字节
            var size = new Size {Width = BitConverter.ToInt16(buffer, 6), Height = BitConverter.ToInt16(buffer, 8)};


            return size;
        }

        private static Size GetJpegSize(byte[] buffer)
        {
            var stream = new MemoryStream(buffer);
            var size = new Size();
            //读取2个字节的SOI，即0xFFD8
            var header = new byte[2];
            stream.Read(header, 0, 2);
            //判断是否为JPG，不是退出解析
            if (!(header[0] == 0xFF &&
                header[1] == 0xD8))
            {
                //不是JPG图片
                throw new YoudaoNoteException("not jpeg file");
            }

            //段类型
            int type;
            //记录当前读取的位置
            //逐个遍历所以段，查找SOFO段
            do
            {
                int ff;
                do
                {
                    //每个新段的开始标识为oxff，查找下一个新段
                    ff = stream.ReadByte();
                    if (ff < 0) //文件结束
                    {
                        throw new YoudaoNoteException("end of jpeg file");
                    }
                } while (ff != 0xff);

                do
                {
                    //段与段之间有一个或多个oxff间隔，跳过这些oxff之后的字节为段标识
                    type = stream.ReadByte();
                } while (type == 0xff);

                //记录当前位置
                var ps = stream.Position;
                switch (type)
                {
                    case 0x00:
                    case 0x01:
                    case 0xD0:
                    case 0xD1:
                    case 0xD2:
                    case 0xD3:
                    case 0xD4:
                    case 0xD5:
                    case 0xD6:
                    case 0xD7:
                        break;
                    case 0xc0: //SOF0段（图像基本信息）
                    case 0xc2: //JFIF格式的 SOF0段
                        {
                            //找到SOFO段，解析宽度和高度信息
                            size = getJpgSize(stream);
                            break;
                        }
                    default: //别的段都跳过
                             //获取段长度，直接跳过
                        ps = stream.ReadByte() * 256;
                        ps = stream.Position + ps + stream.ReadByte() - 2;
                        break;
                }
                if (ps + 1 >= stream.Length) //文件结束
                {
                    throw new YoudaoNoteException("end of jpeg file");
                }
                stream.Position = ps; //移动指针
            } while (type != 0xda); // 扫描行开始

            return size;
        }

        private static Size GetPngSize(byte[] buffer)
        {
            var stream = new MemoryStream(buffer);
            //读取图片文件头8个字节，并根据这8个字节来判断是否为PNG图片
            var header = new byte[8];
            stream.Read(header, 0, 8);
            //Png图片 8字节：89 50 4E 47 0D 0A 1A 0A
            if (!(header[0] == 0x89 &&
                header[1] == 0x50 && // P
                header[2] == 0x4E && // N
                header[3] == 0x47 && // G
                header[4] == 0x0D &&
                header[5] == 0x0A &&
                header[6] == 0x1A &&
                header[7] == 0x0A))
            {
                //不是PNG图片
                throw new YoudaoNoteException("not png file");
            }

            //数据域长度　 4 　　 指定数据域的长度，固定为00 00 00 0D
            //数据块符号　 4  　　49 48 44 52，是“IHDR”的 Ascii 码
            stream.Seek(8, SeekOrigin.Current);

            //读取宽度，高度 各4字节
            var temp = new byte[8];
            stream.Read(temp, 0, temp.Length);

            Array.Reverse(temp, 0, 4);
            Array.Reverse(temp, 4, 4);

            var size = new Size {Width = BitConverter.ToInt32(temp, 0), Height = BitConverter.ToInt32(temp, 4)};

            return size;
        }

        /// <summary>
        /// 解析JPG图片的尺寸
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static Size getJpgSize(Stream stream)
        {
            var size = new Size();

            //跳过2个自己长度信息和1个字节的精度信息
            stream.Seek(3, SeekOrigin.Current);

            //高度 占2字节 低位高位互换
            size.Height = stream.ReadByte() * 256;
            size.Height += stream.ReadByte();
            //宽度 占2字节 低位高位互换
            size.Width = stream.ReadByte() * 256;
            size.Width += stream.ReadByte();

            return size;
        }
    }
}
