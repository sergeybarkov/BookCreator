using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            string default_path = @"C:\Users\barkov\Documents\ГУЗ\Электронные учебники 2017\Бугаевская_2016\production";  //@"D:\гранты_2015\гальченко\гальченко-production"; //@"D:\головная_боль\липски"; //@"D:\_ГРАНТЫ_2014\приходько_current\_prihodko"; //@"D:\_geodezia"; //@"d:\ebook_output";


            Console.WriteLine("***** Чудо-генератор электронных учебников *****");
            Console.WriteLine();

            //Console.WriteLine("Укажите режим: 1) БД 2) WORD:");
            //var key = Console.ReadKey();


            string indir = String.Empty;

                var indir_default = @"C:\Users\barkov\Documents\ГУЗ\Электронные учебники 2017\Бугаевская_2016\Бугаевская_готовое"; //@"D:\гранты_2015\гальченко\гальченко-word"; //@"C:\Users\barkov\Desktop\Липски\Липски"; //@"D:\_ПАКУНОВА_УЧЕБНИК\NEWNEW\нарезка"; //@"C:\Users\barkov\Desktop\Пакунова_готовое"; //@"D:\_ГРАНТЫ_2014\приходько_current\Приходько"; //@"D:\_ГРАНТЫ_2014\Баранов\подготовленное"; //@"D:\_ГРАНТЫ_2014\Пакунова"; //@"d:\ebook_input";

                Console.WriteLine("Откуда брать файлы? [{0}]", indir_default);
                indir = Console.ReadLine();
                if (String.IsNullOrEmpty(indir))
                {
                    indir = indir_default;
                }


            Console.WriteLine("Куда складывать файлы? [{0}]", default_path);
            string outdir = Console.ReadLine();
            if (String.IsNullOrEmpty(outdir))
            {
                outdir = default_path;
            }

            BookBuilder builder = new BookBuilder(outdir, indir);

            builder.WriteChapters();

            Console.WriteLine("Вроде что-то получилось... [ENTER для выхода]");
            Console.ReadLine();
        }
    }
}
