using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace uyanBerber
{
    class Program
    {
        const int gelenMusteriSayisi =60;
        const int beklemeSandalyesiSayisi = 10;
        const int berberKoltuguSayisi = 5;
        const int berberSayisi = 5;
        static Semaphore uyanmaKapisi = new Semaphore(0, berberSayisi);
        static Semaphore uyumaKapisi = new Semaphore(0, berberSayisi);
        static Semaphore berberKoltugu = new Semaphore(berberKoltuguSayisi, berberKoltuguSayisi);
        static Semaphore tekTekGirilsin = new Semaphore(1, 1);

        static void BerberYapsin()
        {
            while (true)
            {
                Console.WriteLine("bir berber uyuyor");
                uyanmaKapisi.WaitOne();
                Console.WriteLine("bir berber uyandi");
                uyumaKapisi.WaitOne();
            }

        }
        static int doluBeklemeSandalyesi = 0;
        static int doluBerberKoltugu = 0;
        static int musteriSayac = 0;
        static void MusteriYapsin()
        {
            musteriSayac++;
            int a = r.Next(500);
            Thread.Sleep(a);//müşterilerin ara ara gelmesi için musteri treadlerini rasgele uyuttum
            tekTekGirilsin.WaitOne();//yani içerde 19 kişi varken kapıdan aynı anda 5 kişi girip...
            Console.WriteLine("dükkana yeni müşteri geldi");
            if (doluBeklemeSandalyesi < 20)
            {
                if (doluBerberKoltugu<5 && doluBeklemeSandalyesi == 0)
                {
                    uyanmaKapisi.Release();
                }
                tekTekGirilsin.Release();//...bekleme salonunu 24 kişiye çıkarmasın
                doluBeklemeSandalyesi++;
                Console.WriteLine("bekleme odasında bekleyen müşteri sayısı " + doluBeklemeSandalyesi + " tane oldu");
                berberKoltugu.WaitOne();//bekleme salonundan 5'ten fazla kişinin burada berber koltuğuna geçmesi engellenir
                doluBerberKoltugu++;
                Console.WriteLine("bekleme odasından berber koltuğuna bir müşteri geçti");
                doluBeklemeSandalyesi--;
                Thread.Sleep(a);//traş süresi olsun
                berberKoltugu.Release();
                doluBerberKoltugu--;
                Console.WriteLine("bir müşterinin traşı bitti");
                if (doluBerberKoltugu<5 && doluBeklemeSandalyesi==0)
                {
                    uyumaKapisi.Release();
                }
                
            }
            else
            {
                Console.WriteLine("yeni gelen müşteri bekleme salonunda yer bulamadı ve gidiyor.");
                tekTekGirilsin.Release();//deadlock olmaması için
            }
            if (musteriSayac == gelenMusteriSayisi)
            {
                Console.ReadLine();
            }

        }

        static Random r;
        static void Main(string[] args)
        {
            r = new Random();
            Thread[] berber = new Thread[berberSayisi];
            for (int i = 0; i < berberSayisi; i++)
            {
                berber[i] = new Thread(() => BerberYapsin());
                berber[i].Start();
            }
            
            Thread[] musteri = new Thread[gelenMusteriSayisi];
            for (int i = 0; i < gelenMusteriSayisi; i++)
            {
                musteri[i] = new Thread(() => MusteriYapsin());
                musteri[i].Start();
            }

        }
    }
}
