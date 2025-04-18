
namespace Game.Tutorial
{
    public class PlayerStats : Stats
    {
        public void AddStats(float health, float stVatLy, float stPhep, float giapVatLy, float giapPhep,
                     float tocDoDanh, float tocDoDiChuyen, float tiLeChiMang, float stChiMang)
        {
            this.currentHealth += health;
            this.stVatLy += stVatLy;
            this.stPhep += stPhep;
            this.giapVatLy += giapVatLy;
            this.giapPhep += giapPhep;
            this.tocDoDanh += tocDoDanh;
            this.tocDoDiChuyen += tocDoDiChuyen;
            this.tiLeChiMang += tiLeChiMang;
            this.stChiMang += stChiMang;
            UpdateHealthBar();
        }
    }
}
