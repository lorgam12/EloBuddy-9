﻿using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;
using System.Linq;


namespace KGragas
{

   
    internal class ModesManager
    {
       

        public static void Combo()
        {
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;
            var alvo = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            var predPosQ = Prediction.Position.PredictLinearMissile(alvo, Q.Range, Q.Width, Q.CastDelay, Q.Speed, int.MaxValue, null, false);
            var predPos = Prediction.Position.PredictLinearMissile(alvo, R.Range, R.Width, R.CastDelay, R.Speed, int.MaxValue, null, false);
            var predPosE = Prediction.Position.PredictLinearMissile(alvo, E.Radius, E.Width, E.CastDelay, E.Speed, 0, null, true);
            if (!alvo.IsValid()) return;

            if (Q.IsReady() && alvo.IsValidTarget(Q.Range) && Program.ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast(predPosQ.CastPosition);
                QLogic.CastedQ = true;
            }

            if (W.IsReady() && alvo.IsValidTarget(E.Range) && Program.ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue)
            {
                W.Cast();

            }
            if (E.IsReady() && alvo.IsValidTarget(E.Range) && Program.ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(alvo);


            }
            if (R.IsReady() && alvo.IsValidTarget(R.Range) && Program.ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue )//&& !(Q.IsInRange(alvo)))
            {
                R.Cast(predPos.CastPosition + 100);

            }
            if (E.IsReady() && alvo.IsValidTarget(R.Range) && !(Q.IsInRange(alvo)))
            {
                E.Cast(predPosE.CastPosition);

            }


        }

        public static void Harass()
        {

            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;
            var alvo = TargetSelector.GetTarget(Program.R.Range, DamageType.Magical);
            var predPos = Prediction.Position.PredictLinearMissile(alvo, Program.R.Range, Program.R.Width, Program.R.CastDelay, Program.R.Speed, int.MaxValue, null, false);
            if (!alvo.IsValid()) return;
            if ((Program._Player.ManaPercent <= Program.ModesMenu1["ManaH"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (Q.IsReady() && alvo.IsValidTarget(Q.Range) && Program.ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue && (Program.CastedQ = false))
            {
                Q.Cast(Q.GetPrediction(alvo).CastPosition);
                Program.CastedQ = true;
            }

            if (W.IsReady() && alvo.IsValidTarget(E.Range) && Program.ModesMenu1["HarassW"].Cast<CheckBox>().CurrentValue)
            {
                W.Cast();

            }
            if (E.IsReady() && alvo.IsValidTarget(E.Range) && Program.ModesMenu1["HarassE"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(alvo);


            }
        }

        public static void LaneClear()
        {
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range));
            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, E.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (minions == null) return;
            if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaF"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            
            if (Q.IsReady() && Program.Q.IsInRange(minions) && Program.ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue && (minion >= Program.ModesMenu2["MinionQ"].Cast<Slider>().CurrentValue))
            {
               
                    Q.Cast(Q.GetPrediction(minions).CastPosition);

                    Program.CastedQ = true;
                
            }

            if (W.IsReady() && E.IsInRange(minions) && Program.ModesMenu2["FarmW"].Cast<CheckBox>().CurrentValue)
            {
                W.Cast();

            }
            if (E.IsReady() && Program.E.IsInRange(minions) && Program.ModesMenu2["FarmE"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(minions);

            }

        }

            public static void LastHit()
        {
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;
            var qminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Program.Q.Range) && (DamageLib.QCalc(m) > m.Health));
            var eminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Program.E.Range) && (DamageLib.ECalc(m) > m.Health));
            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Program.E.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (qminions == null) return;
            var prediction = Program.Q.GetPrediction(qminions);
            if (Q.IsReady() && Program.Q.IsInRange(qminions) && Program.ModesMenu2["LastQ"].Cast<CheckBox>().CurrentValue && qminions.Health < DamageLib.QCalc(qminions))
                
                    Q.Cast(Q.GetPrediction(qminions).CastPosition);

                    Program.CastedQ = true;
                
            if (Program.E.IsReady() && Program.E.IsInRange(eminions) && Program.ModesMenu2["LastE"].Cast<CheckBox>().CurrentValue && eminions.Health < DamageLib.ECalc(eminions))
            {
                E.Cast(eminions);
            }



        }
        
            public static void JungleClear()
            {
                var Q = Program.Q;
                var W = Program.W;
                var E = Program.E;
                var R = Program.R;

                var jungleMonsters = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(Program.Q.Range));
                var minioon = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Program.E.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
                if (jungleMonsters == null) return;
                if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaF"].Cast<Slider>().CurrentValue))
                {
                    return;
                }
                if (Q.IsReady() && Q.IsInRange(jungleMonsters) && Program.ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue)
                  
                        Q.Cast(Q.GetPrediction(jungleMonsters).CastPosition);

                        Program.CastedQ = true;
                    

                if (W.IsReady() && E.IsInRange(jungleMonsters) )//&&  Program.ModesMenu2["FarmW"].Cast<CheckBox>().CurrentValue)
                {
                    W.Cast();

                }
                if (E.IsReady() && E.IsInRange(jungleMonsters)) //&& Program.ModesMenu1["FarmE"].Cast<CheckBox>().CurrentValue)
                {
                    E.Cast(jungleMonsters);

                }
            }




       





        }
    }

