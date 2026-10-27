using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace proyectoArenaElemental
{
    public class MotorCombate
    {
        // ESTRUCTURAS DINÁMICAS REQUERIDAS //
               private Queue<Personaje> colaEquipoJugador;

        
        private Stack<string> pilaHistorialEfectos;

        // ATRIBUTOS DE TURNO //
        public int EnergiaActual { get; private set; }
        public int TurnoActual { get; private set; }
        public Personaje PersonajeActivo { get; private set; }

        // TEMPORIZADOR (TIMER) //
        private Timer timerTurno;
        public int TiempoRestanteTurno { get; private set; }
        private const int TIEMPO_LIMITE_SEGUNDOS = 30; 

        
        public event Action<string> OnActualizarUI;
        public event Action<string> OnTurnoAgotado;
        public event Action<int> OnTiempoTick;

        
        public class MotorCombate
        {
            // ESTRUCTURAS DINÁMICAS REQUERIDAS //
            
            private Queue<Personaje> colaEquipoJugador;

            
            private Stack<string> pilaHistorialEfectos;

            // ATRIBUTOS DE TURNO //
            public int EnergiaActual { get; private set; }
            public int TurnoActual { get; private set; }
            public Personaje PersonajeActivo { get; private set; }

            // TEMPORIZADOR (TIMER) ///
            private Timer timerTurno;
            public int TiempoRestanteTurno { get; private set; }
            private const int TIEMPO_LIMITE_SEGUNDOS = 30; 

            
            public event Action<string> OnActualizarUI;
            public event Action<string> OnTurnoAgotado;
            public event Action<int> OnTiempoTick;

            
            public MotorCombate(List<Personaje> personajesIniciales)
            {
                if (personajesIniciales == null || personajesIniciales.Count == 0)
                {
                    throw new ArgumentException("El equipo debe contener al menos un personaje.");
                }

                colaEquipoJugador = new Queue<Personaje>(personajesIniciales);
                pilaHistorialEfectos = new Stack<string>();

                TurnoActual = 1;
                EnergiaActual = 2; 
                
                PersonajeActivo = colaEquipoJugador.Dequeue();

                ConfigurarTimer();
            }

            private void ConfigurarTimer()
            {
                timerTurno = new Timer();
                timerTurno.Interval = 1000; 
                timerTurno.Tick += TimerTurno_Tick;
            }

            
            public void IniciarTurno()
            {
                
                if (TurnoActual > 1)
                {
                    EnergiaActual += 1;
                }

                TiempoRestanteTurno = TIEMPO_LIMITE_SEGUNDOS;
                timerTurno.Start();

                string registro = "Turno: {TurnoActual} | Energía disponible: {EnergiaActual} | Activo: {PersonajeActivo.Nombre} ({PersonajeActivo.Elemento})";
                pilaHistorialEfectos.Push(registro);
                OnActualizarUI?.Invoke(registro);
            }

            private void TimerTurno_Tick(object sender, EventArgs e)
            {
                TiempoRestanteTurno--;
                OnTiempoTick.Invoke(TiempoRestanteTurno);

                if (TiempoRestanteTurno <= 0)
                {
                    timerTurno.Stop();
                    OnTurnoAgotado.Invoke("¡El tiempo del turno ha expirado!");
                    FinalizarTurno();
                }
            }

            
            public bool GastarEnergia(int costo, string descripcionAccion)
            {
                if (EnergiaActual >= costo)
                {
                    EnergiaActual -= costo;
                    string registro = "{PersonajeActivo.Nombre} realizó: {descripcionAccion} (-{costo} Energía). Restante: {EnergiaActual}";
                    pilaHistorialEfectos.Push(registro);
                    OnActualizarUI?.Invoke(registro);
                    return true;
                }
                return false; // Energía insuficiente
            }

            
            public void RealizarRelevo()
            {
                // Devolver el personaje actual al fondo de la Cola
                colaEquipoJugador.Enqueue(PersonajeActivo);

                // Sacar al siguiente personaje del frente de la Cola
                PersonajeActivo = colaEquipoJugador.Dequeue();

                string registro = "¡Relevo ejecutado! Entra a la arena: {PersonajeActivo.Nombre} [{PersonajeActivo.Elemento}]";
                pilaHistorialEfectos.Push(registro);
                OnActualizarUI.Invoke(registro);
            }

            
            public string ObtenerUltimoHistorial()
            {
                if (pilaHistorialEfectos.Count > 0)
                {
                    return pilaHistorialEfectos.Peek();
                }
                return "No hay eventos recientes en el historial.";
            }

            
            public void FinalizarTurno()
            {
                timerTurno.Stop();
                TurnoActual++;
                IniciarTurno();
            }
        }
    }
}