using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Autopeli
{
    /// @author luka.rantalainen
    /// @version 18.01.2024
    /// <summary>
    /// 
    /// </summary>
    public class Autopeli : PhysicsGame
    {
        PhysicsObject auto;
        PhysicsObject maali;
        IntMeter pistelaskuri;
        IntMeter aikalaskuri;
        PhysicsObject checkpoint;
        Image palikanKuva = LoadImage("nuoli");
        Image autonKuva = LoadImage("car (1)");

        bool checkpointOhitettu = false;
        public override void Begin()
        {
            aloitaAlusta();
            PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
            Level.Background.Color = Color.DarkGray;
        }
        void aloitaAlusta()
        {
            ClearAll();
            LuoPelaaja();
            LisaaNappaimet();
            LuoAikalaskuri();
            luokartta();
            LuoMaali();
            LuoCheckpoint();
            AddCollisionHandler(auto, maali, PelaajatTormaavat);
            AddCollisionHandler(auto, checkpoint, CheckpointOhitettu);
            LuoPistelaskuri();
            MediaPlayer.Play("taustamusiikki");
            MediaPlayer.IsRepeating = true;
        }
        void LuoPelaaja()
        { auto = new PhysicsObject(50,100);
            auto.Image = autonKuva;
            Add(auto);
            auto.LinearDamping = 2;
            auto.AngularDamping = 3;
            auto.X = 375;
            auto.Y = 0;
        }

        void LisaaNappaimet()
        {
            Keyboard.Listen(Key.W, ButtonState.Down, LiikuEteenpain, "Liiku eteenpäin");
            Keyboard.Listen(Key.S, ButtonState.Down, LiikuTaaksepain, "Liiku taaksepäin");
            Keyboard.Listen(Key.A, ButtonState.Down, KaannyVasemmalle, "Liiku vasemmalle");
            Keyboard.Listen(Key.D, ButtonState.Down, KaannyOikealle, "Liiku oikealle");
            Keyboard.Listen(Key.R, ButtonState.Pressed, aloitaAlusta, "Aloita alusta");
            Keyboard.Listen(Key.M, ButtonState.Pressed, Mute, "Äänet pois");
            Keyboard.Listen(Key.N, ButtonState.Pressed, unmute, "Äänet päälle");
        }

        void Mute()
        {
            MediaPlayer.IsMuted = true;
        }

        void unmute()
        {
            MediaPlayer.IsMuted = false;
        }

        void LiikuEteenpain()
        {
            Vector eteen = Vector.FromLengthAndAngle(7000, auto.Angle + Angle.FromDegrees(90));

            auto.Push(eteen);

        }



        void LiikuTaaksepain()
        {
            Vector eteen = Vector.FromLengthAndAngle(-3000, auto.Angle + Angle.FromDegrees(90));

            auto.Push(eteen);
        }
        
        void KaannyVasemmalle()
        {
            auto.ApplyTorque(0.09);
        }

        void KaannyOikealle()
        {
            auto.ApplyTorque(-0.09);
        }

        void luokartta()
        { TileMap ruudut = TileMap.FromLevelAsset("kartta");

            ruudut.SetTileMethod('1', LuoValkoinenPalikka);
            ruudut.SetTileMethod('2', GreenBlock);
            ruudut.SetTileMethod('3', LuoEste);
            ruudut.SetTileMethod('4', LuoNuoli);
            ruudut.SetTileMethod('5', LuoPunainenPalikka);
            ruudut.Execute();
        }


        void LuoValkoinenPalikka(Vector paikka, double leveys, double korkeus)
        {
            GameObject palikka = PhysicsObject.CreateStaticObject(30, 30);
            palikka.Position = paikka;
            palikka.Shape = Shape.Rectangle;
            palikka.Color = Color.White;
            Add(palikka);

        }

        void LuoPunainenPalikka(Vector paikka, double leveys, double korkeus)
        {
            GameObject palikka = PhysicsObject.CreateStaticObject(30, 30);
            palikka.Position = paikka;
            palikka.Shape = Shape.Rectangle;
            palikka.Color = Color.Red;
            Add(palikka);
        }

        void GreenBlock(Vector paikka, double leveys, double korkeus)
        {
            GameObject palikka = PhysicsObject.CreateStaticObject(29, 29);
            palikka.Position = paikka;
            palikka.Shape = Shape.Rectangle;
            palikka.Color = Color.Green;
            Add(palikka, -2);

        }

        void LuoEste(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject palikka = PhysicsObject.CreateStaticObject(28, 28);
            palikka.Position = paikka;
            palikka.Shape = Shape.Circle;
            palikka.Color = Color.Red;
            Add(palikka);
        }

        void LuoNuoli(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject palikka = PhysicsObject.CreateStaticObject(300, 300);
            palikka.Position = paikka;
            palikka.Image = palikanKuva;
            palikka.IgnoresCollisionResponse = true;
            Add(palikka, -3);
        }

        void LuoMaali()
        {
            maali = new PhysicsObject(295, 10);
            maali.Color = Color.White;
            maali.IgnoresCollisionResponse = true;
            maali.X = 335;
            maali.Y = 0;
            Add(maali, -3);
               
        }

        void LuoCheckpoint()
        {
            checkpoint = new PhysicsObject(287, 10);
            checkpoint.Color = Color.Transparent;
            checkpoint.IgnoresCollisionResponse = true;
            checkpoint.X = -355;
            checkpoint.Y = 0;
            Add(checkpoint, -3);

        }


        void PelaajatTormaavat(PhysicsObject tormaaja, PhysicsObject kohde)
        {
            

            if (checkpointOhitettu)
            {
                MessageDisplay.Add("Kierros");
                pistelaskuri.Value += 1;
                checkpointOhitettu = false;
            }

        }

        void CheckpointOhitettu(PhysicsObject tormaaja, PhysicsObject kohde)
        {
            checkpointOhitettu = true;
        
        }

        void LuoPistelaskuri()
        {
            pistelaskuri = new IntMeter(0);
                
            Label pistenaytto = new Label();
            pistenaytto.X = Screen.Left + 100;
            pistenaytto.Y = Screen.Top - 100;
            pistenaytto.TextColor = Color.Black;
            pistenaytto.Color = Color.White;
            pistelaskuri.MaxValue = 2;
            pistelaskuri.UpperLimit += Maali;
            pistenaytto.BindTo(pistelaskuri);
            Add(pistenaytto);
        }



        void Maali()
        {
 
            
            MessageDisplay.Add("Maali!");
            aikalaskuri.Stop();
        }

        public void LuoAikalaskuri()
        {
            Timer aikalaskuri = new Timer();
            aikalaskuri.Start();

            Label aikanaytto = new Label();
            aikanaytto.TextColor = Color.White;
            aikanaytto.DecimalPlaces = 1;
            aikanaytto.X = Screen.Left + 500;
            aikanaytto.Y = Screen.Top + 100;
            aikanaytto.BindTo(aikalaskuri.SecondCounter);
            Add(aikanaytto);
        }

    }
}