using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;


namespace Ompelija 
    /// @author Minna Närhi
    /// @version 06.06.2024
    /// <summary>
    /// Pelissä ruutuun ilmestyy kuva käsityövälineestä. Lisäksi kuvaruudulle ilmestyy neljä sanaa, joista
    /// yksi on kuvassa näkyvän välineen oikea nimi. Pelaajan tehtävänä on klikata hiirellä kuvaa vastaavaa sanaa. 
    /// Jos pelaaja klikkaa väärää sanaa, peli päättyy. Jos pelaaja klikkaa oikeaa sanaa, hän saa yhden pisteen
    /// ja kuvaruutuun ilmestyy seuraava kuva ja sanat.
    /// Peli on läpäisty onnistuneesti, kun pelaaja on saanut kerättyä viisi pistettä.
    ///  </summary>

{
    public class Ompelija : PhysicsGame
    {
        // Alustetaan koodissa käytettäviä muuttujia
        public int nykyinenKuvaIndex;
        public List<Label> labelit;
        public GameObject nykyinenKuva;
        public Dictionary<string, List<string>> kuvatJaVihjeet;
        public Dictionary<string, string> oikeatVastaukset;
        public int pisteet;

        public override void Begin()
        {
            Gravity = new Vector(0, -1000);
            
            // Kuvat ja sanavaihtoehdot
            kuvatJaVihjeet = new Dictionary<string, List<string>>
            {
                
                { "hakaneulat.jpg", new List<string>() { "Nuppineulat", "Hakaneulat", "Naulat", "Silmäneulat" } },
                { "kaava.jpg", new List<string>() { "Kaava", "Malli", "Suunnitelma", "Työpaperi" } },
                { "ratkoja.jpg", new List<string>() { "Ratkoja", "Poistaja", "Neula", "Härveli" } },
                { "ompelukone.jpg", new List<string>() { "Ompelukone", "Poistaja", "Neula", "Härveli" } },
                { "paininjalka.jpg", new List<string>() { "Paininkäsi", "Paininsormi", "Paininvarvas", "Paininjalka" } }
            };

            // Oikeat vastaukset kutakin kuvaa varten
            oikeatVastaukset = new Dictionary<string, string>
            {
                { "hakaneulat.jpg", "Hakaneulat" },
                { "kaava.jpg", "Kaava" },
                { "ratkoja.jpg", "Ratkoja" },
                { "ompelukone.jpg", "Ompelukone" },
                { "paininjalka.jpg", "Paininjalka" }
            };
            

            // Näytetään ensimmäinen kuva ja sanavaihtoehdot
            nykyinenKuvaIndex = 0;
            pisteet = 0;
            NaytaKuvaJaVaihtoehdot();

            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        }
        
        public void LuoLabelit() 
            // Alustetaan labelit sanavaihtoehdoille
        {
            
            labelit = new List<Label>();
            for (int i = 0; i < 4; i++)
            {
                Label label = new Label("");
                labelit.Add(label);
                Add(label);
            }
        }

        public void PoistaLabelit()
            // Poistetaan vanhat labelit
        {
            foreach (var label in labelit)
            {
                label.Destroy();
            }
        }
        
        public void NaytaKuvaJaVaihtoehdot()
        {
            // Poistetaan edellinen kuva, jos on olemassa
            if (nykyinenKuva != null)
            {
                nykyinenKuva.Destroy();
                nykyinenKuva = null;
            }
            
            var kuvannimi = new List<string>(kuvatJaVihjeet.Keys)[nykyinenKuvaIndex];
            var vaihtoehdot = kuvatJaVihjeet[kuvannimi];

            // Ladataan ja näytetään uusi kuva
            nykyinenKuva = new GameObject(400, 400);
            nykyinenKuva.Image = LoadImage(kuvannimi);
            nykyinenKuva.Position = new Vector(Level.Left + nykyinenKuva.Width, Level.Top - nykyinenKuva.Height);
            Add(nykyinenKuva);
            LuoLabelit();
            
            // Asetetaan sanavaihtoehdot labeleihin
            for (int i = 0; i < labelit.Count; i++)
            {
                labelit[i].Text = vaihtoehdot[i];
                labelit[i].Position = new Vector(Level.Left + (i + 1) * 200, Level.Bottom + 100);
                

                // Oikean vastauksen tarkistus
                if (vaihtoehdot[i] == oikeatVastaukset[kuvannimi])
                {
                    Mouse.ListenOn(labelit[i], MouseButton.Left, ButtonState.Pressed, NaytaOikeinTeksti, null);
                }
                else
                {
                    Mouse.ListenOn(labelit[i], MouseButton.Left, ButtonState.Pressed, NaytaVaarinTeksti, null);
                }
            }
        }

        public void NaytaOikeinTeksti()
            // Luodaan teksti joka ilmestyy ruutuun, jos pelaaja klikkaa oikeaa sanaa.
        {
            Label oikeinTeksti = new Label("Oikein");
            oikeinTeksti.TextColor = Color.Green;
            oikeinTeksti.Font = Font.Default;
            
            oikeinTeksti.Position = new Vector(Level.Right - oikeinTeksti.Width / 2, Level.Top - oikeinTeksti.Height * 2);

            Add(oikeinTeksti);

            Timer.SingleShot(2, delegate
            {
                oikeinTeksti.Destroy();
                pisteet++;
                nykyinenKuvaIndex++;
                PoistaLabelit();
                if (pisteet >= 5)
                {
                    MessageDisplay.Add("Peli läpäisty! Sait 5 pistettä.");
                }
                else
                {
                    NaytaKuvaJaVaihtoehdot();
                }
            });
        }

        public void NaytaVaarinTeksti()
        // Luodaan teksti joka ilmestyy, jos pelaaja klikkaa väärää sanaa ja sen seurauksena peli päättyy.
        {
            MessageDisplay.Add("Väärin! Peli loppui!");
            nykyinenKuva.Destroy();
            PoistaLabelit();
        }
        
    }
}
