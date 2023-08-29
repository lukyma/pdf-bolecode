using MessagePack.Formatters;
using Microsoft.AspNetCore.Mvc.Rendering;
using SkiaSharp;
using System.Drawing;
using System.Drawing.Imaging;

namespace CreatePdf.Helpers
{
    public static class Int2of5
    {
        private static string Encode(string Data)
        {
            try
            {
                //0 = thin
                //1 = thick
                IDictionary<int, string> NumbersMapping = new Dictionary<int, string>
                {
                    { 0, "00110" },
                    { 1, "10001" },
                    { 2, "01001" },
                    { 3, "11000" },
                    { 4, "00101" },
                    { 5, "10100" },
                    { 6, "01100" },
                    { 7, "00011" },
                    { 8, "10010" },
                    { 9, "01010" }
                };

                if (string.IsNullOrEmpty(Data)) throw new Exception("No data received");
                if (!Data.All(char.IsDigit)) throw new Exception("Only numbers are accepted");
                if (Data.Length % 2 != 0) throw new Exception("Number os digits have to be even");

                IList<KeyValuePair<int, string>> Digits = new List<KeyValuePair<int, string>>();
                for (int i = 0; i < Data.Length; i++)
                {
                    int key = Convert.ToInt32(Data[i].ToString());
                    string value = NumbersMapping[Convert.ToInt32(Data[i].ToString())];

                    Digits.Add(new KeyValuePair<int, string>(Convert.ToInt32(Data[i].ToString()),
                               NumbersMapping[Convert.ToInt32(Data[i].ToString())]));
                }

                string Result = string.Empty;
                for (int i = 0; i < Digits.Count; i += 2)
                {
                    string Pair1 = Digits[i].Value;
                    string Pair2 = Digits[i + 1].Value;

                    //Pair 1 e 2 will get interleaved
                    //Pair 1 = will be bars
                    //Pair 2 = will be spaces
                    //Pseudo-codes:
                    //A = thin space
                    //B = thick space
                    //X = thin bar
                    //Y = thick bar
                    for (int j = 0; j < 5; j++)
                        Result += (Pair1[j].ToString() == "0" ? "X" : "Y") +
                                  (Pair2[j].ToString() == "0" ? "A" : "B");
                }

                //Append start and ending
                return "XAXA" + Result + "YAX";
            }
            catch (Exception ex)
            {
                return "#" + ex.Message;
            }
        }

        public static SKData GenerateBarCode(string Data, int Width, int Height, float ScaleFactor)
        {
            try
            {
                string EncodedData = Encode(Data);
                if (string.IsNullOrEmpty(EncodedData))
                    throw new Exception("Encoding process returned empty");
                if (EncodedData[0].ToString() == "#") throw new Exception(EncodedData);

                float Position = 20, ThinWidth = 1 * ScaleFactor, ThickWidth = 3 * ScaleFactor;

                using SKSurface surface = SKSurface.Create(new SKImageInfo(Width, Height));

                using SKCanvas canvas = surface.Canvas;

                using var paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.White
                };
                //var rect = SKRect.Create(0, 0, Width, Height);
                //Initial white color filling
                canvas.DrawRect(0, 0, Width, Height, paint);

                for (int i = 0; i < EncodedData.Length; i++)
                {
                    //Replace the pseudo-codes with bars or spaces
                    //canvas.Clear();
                    switch (EncodedData[i].ToString())
                    {
                        case "A":
                            {
                                using var paint1 = new SKPaint
                                {
                                    Style = SKPaintStyle.Fill,
                                    Color = SKColors.White
                                };
                                canvas.DrawRect(Position, 0, ThinWidth, Height, paint1);
                                Position += ThinWidth;
                            }
                            break;
                        case "B":
                            {
                                using var paint1 = new SKPaint
                                {
                                    Style = SKPaintStyle.Fill,
                                    Color = SKColors.White
                                };
                                canvas.DrawRect(Position, 0, ThickWidth, Height, paint1);
                                Position += ThickWidth;
                            }
                            break;
                        case "X":
                            {
                                using var paint1 = new SKPaint
                                {
                                    Style = SKPaintStyle.Fill,
                                    Color = SKColors.Black
                                };
                                canvas.DrawRect(Position, 0, ThinWidth, Height, paint1);
                                Position += ThinWidth;
                            }
                            break;
                        case "Y":
                            {
                                using var paint1 = new SKPaint
                                {
                                    Style = SKPaintStyle.Fill,
                                    Color = SKColors.Black
                                };
                                canvas.DrawRect(Position, 0, ThickWidth, Height, paint1);
                                Position += ThickWidth;
                            }
                            break;
                    }
                    canvas.Flush();
                }
                return surface.Snapshot().Encode(SKEncodedImageFormat.Png, 90);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SKData GenerateBarCode3(string Data, int Width, int Height, float ScaleFactor)
        {
            try
            {
                int width = Width;
                int height = Height;

                // Criando um objeto SKSurface para desenhar o código de barras
                using SKSurface surface = SKSurface.Create(new SKImageInfo(width, height));

                // Obtendo o objeto SKCanvas do objeto SKSurface
                using SKCanvas canvas = surface.Canvas;

                // Definindo as propriedades do pincel para desenhar o código de barras
                using SKPaint paint = new SKPaint();
                paint.Color = SKColors.Black;
                paint.Style = SKPaintStyle.Fill;

                // Desenhando as barras do código de barras
                int x = 0;
                for (int i = 0; i < Data.Length; i += 2)
                {
                    // Obtendo o par de dígitos a partir da string de dados do código de barras
                    string digitPair = Data.Substring(i, 2);

                    // Desenhando a barra correspondente a cada par de dígitos
                    for (int j = 0; j < digitPair.Length; j++)
                    {
                        int barWidth = digitPair[j] == '1' ? 2 : 1;
                        canvas.DrawRect(x, 0, barWidth, height, paint);
                        x += barWidth;
                    }
                }

                // Salvando o código de barras como um arquivo PNG
                using var image = surface.Snapshot();
                return image.Encode(SKEncodedImageFormat.Png, 100);
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }

        public static Image GenerateBarCode2(string Data, int Width, int Height, int ScaleFactor)
        {
            try
            {
                string EncodedData = Encode(Data);
                if (string.IsNullOrEmpty(EncodedData))
                    throw new Exception("Encoding process returned empty");
                if (EncodedData[0].ToString() == "#") throw new Exception(EncodedData);

                int Position = 20, ThinWidth = 1 * ScaleFactor, ThickWidth = 3 * ScaleFactor;
                Image img = new System.Drawing.Bitmap(Width, Height);
                using (Graphics gr = Graphics.FromImage(img))
                {
                    //Initial white color filling
                    gr.FillRectangle(Brushes.White, 0, 0, Width, Height);

                    for (int i = 0; i < EncodedData.Length; i++)
                    {
                        //Replace the pseudo-codes with bars or spaces
                        switch (EncodedData[i].ToString())
                        {
                            case "A":
                                gr.FillRectangle(System.Drawing.Brushes.White,
                                                 Position, 0, ThinWidth, Height);
                                Position += ThinWidth;
                                break;
                            case "B":
                                gr.FillRectangle(System.Drawing.Brushes.White,
                                                 Position, 0, ThickWidth, Height);
                                Position += ThickWidth;
                                break;
                            case "X":
                                gr.FillRectangle(System.Drawing.Brushes.Black,
                                                 Position, 0, ThinWidth, Height);
                                Position += ThinWidth;
                                break;
                            case "Y":
                                gr.FillRectangle(System.Drawing.Brushes.Black,
                                                 Position, 0, ThickWidth, Height);
                                Position += ThickWidth;
                                break;
                        }
                    }
                    return img;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public static class BoletoHelper
    {
        public static string GenerateBarCodeBase64_BarcodeLib(this IHtmlHelper htmlHelper, string number)
        {
            BarcodeLib.Barcode barcode = new();
            barcode.Encode(BarcodeLib.TYPE.Interleaved2of5, number, 1000, 80);

            var base64 = Convert.ToBase64String(barcode.GetImageData(BarcodeLib.SaveTypes.PNG));

            return $"data:image/png;base64,{base64}";
        }

        public static string GenerateBarCodeBase64(this IHtmlHelper htmlHelper, string number)
        {
            BarcodeLib.Barcode barcode = new();
            barcode.Alignment = BarcodeLib.AlignmentPositions.LEFT;
            using var image = Int2of5.GenerateBarCode2(number, 1000, 100, 2);

            using MemoryStream ms = new MemoryStream();

            image.Save(ms, ImageFormat.Png);

            var base64 = Convert.ToBase64String(ms.ToArray());

            return $"data:image/png;base64,{base64}";
        }

        public static string GenerateBarCodeBase64_SkiaSharp(this IHtmlHelper htmlHelper, string number)
        {
            BarcodeLib.Barcode barcode = new();
            barcode.Alignment = BarcodeLib.AlignmentPositions.LEFT;
            using var image = Int2of5.GenerateBarCode(number, 1000, 100, 2);

            var base64 = Convert.ToBase64String(image.ToArray());

            return $"data:image/png;base64,{base64}";
        }
    }
}
