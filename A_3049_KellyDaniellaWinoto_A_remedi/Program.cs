public class Sekolah
    {
        public string idSekolah;

        public string namaSekolah;
        public int jumlahPaketDiterima;

        public Sekolah(string idSekolah, string namaSekolah)
        {
            this.idSekolah = idSekolah;
            this.namaSekolah = namaSekolah;
            this.jumlahPaketDiterima = 0;
        }

        public void TambahPaketDiterima(int jumlah)
        {
            if (jumlah > 0)
            {
                jumlahPaketDiterima += jumlah;
            }
        }

        public void TampilkanInfo()
        {
            Console.WriteLine($"ID Sekolah     : {idSekolah}");
            Console.WriteLine($"Nama Sekolah   : {namaSekolah}");
            Console.WriteLine($"Paket Diterima : {jumlahPaketDiterima} paket");
        }
    }

    public abstract class PaketMBG
    {
        private string idPaket;
        private string namaPaket;
        private int stokDapur;

        public string IdPaket
        {
            get { return idPaket; }
            set { idPaket = value; }
        }

        public string NamaPaket
        {
            get { return namaPaket; }
            set { namaPaket = value; }
        }

        public int StokDapur
        {
            get { return stokDapur; }
            private set
            {
                if (value < 0)
                {
                    stokDapur = 0;
                }
                else
                {
                    stokDapur = value;
                }
            }
        }

        public PaketMBG(string idPaket, string namaPaket, int stokAwal)
        {
            IdPaket = idPaket;
            NamaPaket = namaPaket;
            StokDapur = stokAwal < 0 ? 0 : stokAwal;
        }

        public bool KurangiStok(int jumlah)
        {
            if (jumlah <= 0)
            {
                Console.WriteLine("[Validasi Stok] Jumlah pengurangan harus lebih dari 0.");
                return false;
            }

            if (jumlah > stokDapur)
            {
                Console.WriteLine($"[Validasi Stok] Stok tidak mencukupi! Tersedia: {stokDapur}, Diminta: {jumlah}.");
                return false;
            }

            stokDapur -= jumlah;
            return true;
        }

        public abstract double HitungBiaya(int jumlah);

        public virtual void TampilInfoPaket()
        {
            Console.WriteLine($"[{IdPaket}] {NamaPaket} | Stok Dapur: {StokDapur} paket");
        }
    }

    public class PaketReguler : PaketMBG
    {
        private const double Harga_Reguler = 15000;
        public string MenuUtama { get; set; }

        public PaketReguler(string idPaket, string namaPaket, int stokAwal)
            : base(idPaket, namaPaket, stokAwal)
        {
            MenuUtama = "Nasi, Ayam, Sayur, dan Buah";
        }

        public override double HitungBiaya(int jumlah)
        {
            return jumlah * Harga_Reguler;
        }

        public override void TampilInfoPaket()
        {
            base.TampilInfoPaket();
            Console.WriteLine($"   Jenis: Reguler | Harga: Rp{Harga_Reguler:N0} | Isi: {MenuUtama}");
        }
    }

    public class PaketKhusus : PaketMBG
    {
        private const double Harga_Khusus = 20000;
        public string JenisDiet { get; set; }

        public PaketKhusus(string idPaket, string namaPaket, int stokAwal, string jenisDiet)
            : base(idPaket, namaPaket, stokAwal)
        {
            JenisDiet = jenisDiet;
        }
        public override double HitungBiaya(int jumlah)
        {
            return jumlah * Harga_Khusus;
        }

        public override void TampilInfoPaket()
        {
            base.TampilInfoPaket();
            Console.WriteLine($"   Jenis: Khusus ({JenisDiet}) | Harga: Rp{Harga_Khusus:N0}");
        }
    }

    public interface DapatDidistribusikan
    {
        void KirimPaket();
    }

    public class DistribusiMBG : DapatDidistribusikan
    {
        public Sekolah SekolahTujuan { get; private set; }
        public PaketMBG PaketDikirim { get; private set; }
        public int Jumlah { get; private set; }

        public DistribusiMBG(Sekolah sekolahTujuan, PaketMBG paketDikirim, int jumlah)
        {
            SekolahTujuan = sekolahTujuan;
            PaketDikirim = paketDikirim;
            Jumlah = jumlah;
        }

        public void KirimPaket()
        {
            Console.WriteLine($"\nMemproses Distribusi ke {SekolahTujuan.namaSekolah}");

            if (Jumlah <= 0)
            {
                Console.WriteLine("Distribusi MBG gagal! Alasan: Jumlah paket yang dikirim harus lebih dari 0.");
                return;
            }

            if (Jumlah > PaketDikirim.StokDapur)
            {
                Console.WriteLine($"Distribusi MBG gagal! Alasan: Stok dapur untuk '{PaketDikirim.NamaPaket}' tidak mencukupi (Tersedia: {PaketDikirim.StokDapur}, Diminta: {Jumlah}).");
                return;
            }

            bool berhasilKurangiStok = PaketDikirim.KurangiStok(Jumlah);
            if (berhasilKurangiStok)
            {
                SekolahTujuan.TambahPaketDiterima(Jumlah);
                double totalBiaya = PaketDikirim.HitungBiaya(Jumlah);

                Console.WriteLine($"Distribusi MBG berhasil! Sebanyak {Jumlah} paket makanan telah dikirim ke sekolah {SekolahTujuan.namaSekolah}.");
                Console.WriteLine($"Total Estimasi Biaya Produksi: Rp{totalBiaya:N0}");
            }
        }
    }

    public class PengelolaMBG
    {
        private List<Sekolah> daftarSekolah;
        private List<PaketMBG> daftarPaket;

        public PengelolaMBG()
        {
            daftarSekolah = new List<Sekolah>();
            daftarPaket = new List<PaketMBG>();
        }

        public void TambahSekolah(Sekolah sekolah)
        {
            if (sekolah != null)
            {
                daftarSekolah.Add(sekolah);
                Console.WriteLine($"Sekolah '{sekolah.namaSekolah}' berhasil didaftarkan.");
            }
        }

        public void TambahPaket(PaketMBG paket)
        {
            if (paket != null)
            {
                daftarPaket.Add(paket);
                Console.WriteLine($"Paket '{paket.NamaPaket}' berhasil ditambahkan ke dapur.");
            }
        }

        public void TampilkanDataMBG()
        {
            Console.WriteLine("LAPORAN DATA PROGRAM MBG");

            Console.WriteLine("\nDAFTAR SEKOLAH PENERIMA");
            foreach (var sekolah in daftarSekolah)
            {
                sekolah.TampilkanInfo();
            }

            Console.WriteLine("\nDAFTAR STOK PAKET MAKANAN DAPUR");
            foreach (var paket in daftarPaket)
            {
                paket.TampilInfoPaket();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PengelolaMBG pengelola = new PengelolaMBG();

            Sekolah sekolah1 = new Sekolah("SEK-001", "SDN Nusantara 01");
            Sekolah sekolah2 = new Sekolah("SEK-002", "SMP Harapan Bangsa");
            Sekolah sekolah3 = new Sekolah("SEK-003", "SMA Merah Putih");

            pengelola.TambahSekolah(sekolah1);
            pengelola.TambahSekolah(sekolah2);
            pengelola.TambahSekolah(sekolah3);

            PaketReguler paketReguler = new PaketReguler("PKT-REG", "Paket Gizi Seimbang Harian", 500);
            PaketKhusus paketKhusus = new PaketKhusus("PKT-KHS", "Paket Gizi Spesial", 100, "Rendah Gula & Bebas Gluten");

            pengelola.TambahPaket(paketReguler);
            pengelola.TambahPaket(paketKhusus);

            Console.WriteLine("\nSIMULASI PERHITUNGAN BIAYA");
            PaketMBG polyPaket1 = paketReguler;
            PaketMBG polyPaket2 = paketKhusus;

            int sampelJumlah = 50;
            Console.WriteLine($"Biaya {sampelJumlah} {polyPaket1.NamaPaket} : Rp{polyPaket1.HitungBiaya(sampelJumlah):N0}");
            Console.WriteLine($"Biaya {sampelJumlah} {polyPaket2.NamaPaket} : Rp{polyPaket2.HitungBiaya(sampelJumlah):N0}");

            Console.WriteLine("\nDISTRIBUSI PAKET MBG");

            DapatDidistribusikan d1 = new DistribusiMBG(sekolah1, paketReguler, 200);
            d1.KirimPaket();

            DapatDidistribusikan d2 = new DistribusiMBG(sekolah2, paketKhusus, 40);
            d2.KirimPaket();

            DapatDidistribusikan d3 = new DistribusiMBG(sekolah3, paketReguler, 0);
            d3.KirimPaket();

            DapatDidistribusikan d4 = new DistribusiMBG(sekolah3, paketKhusus, 80);
            d4.KirimPaket();

            pengelola.TampilkanDataMBG();
        }
    }