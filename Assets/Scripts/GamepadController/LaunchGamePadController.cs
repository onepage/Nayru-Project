using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using System.Reflection;
//using Microsoft.DirectX.DirectInput;
//using System;

public class LaunchGamePadController : MonoBehaviour
{
    GamePadController gamePadController = new GamePadController();
    public bool DebugMode = true;

    public GamePadController.DebugGamePad DebugParameters;

    private static float[] _vibrationTimers = new float[4];
    private bool[] _controllersConnected = new bool[4];

    void Awake()
    {
        gamePadController.Start();
        _controllersConnected = GamePadController.ControllerConnected;
    }

    void FixedUpdate()
    {
        GamePadController.DebugMode = DebugMode;
        gamePadController.Update();

        _vibrationTimers = GamePadController.VibrationTimers;
        for (int i = 0; i < _vibrationTimers.Length; i++)
        {
            if (_vibrationTimers[i] > 0 && _vibrationTimers != null)
            {
                var timer = _vibrationTimers[i];
                StartCoroutine(ResetTimer(i, timer));
                _vibrationTimers[i] = -1f;
                GamePadController.VibrationTimers = _vibrationTimers;
            }
        }
        if(DebugMode)
            DebugParameters.Update();
    }

    IEnumerator ResetTimer(int controllerId, float timer)
    {
        yield return new WaitForSeconds(timer);

        gamePadController.StopControllerVibration(controllerId);
    }
}

public class GamePadController
{

    public static bool[] ControllerConnected = new bool[4];

    public static bool DebugMode { private get; set; }

    public static GamePadState[] GamePadStateArray = new GamePadState[4];
    public static PlayerIndex[] PlayerIndexArray = new PlayerIndex[4];
    protected static Controller[] GamePads = new Controller[4];
    public static float[] VibrationTimers = new float[4];

    public static Controller GamePadOne;
    public static Controller GamePadTwo;
    public static Controller GamePadThree;
    public static Controller GamePadFour;
    
    //private static string[] _controllerNames = new string[] {"One", "Two", "Three", "Four"};
    private string[] _abxy = new string[] { "A", "B", "X", "Y", "L3", "R3", "start", "select", "LB", "RB"};
    private string[] _abxyXinput = new string[] { "A", "B", "X", "Y", "LeftStick", "RightStick", "Start", "Back", "LeftShoulder", "RightShoulder"};
    private string[] _dPad = new string[]{"Up", "Down", "Left", "Right"};

    public void Start()
    {
        PlayerIndexArray[0] = PlayerIndex.One;
        PlayerIndexArray[1] = PlayerIndex.Two;
        PlayerIndexArray[2] = PlayerIndex.Three;
        PlayerIndexArray[3] = PlayerIndex.Four;

        for (int i = 0; i < 4; i++)
        {
            GamePadStateArray[i] = GamePad.GetState(PlayerIndexArray[i]);
            GamePads[i] = new Controller(i);
        }
        GamePadOne = GamePads[0];
        GamePadTwo = GamePads[1];
        GamePadThree = GamePads[2];
        GamePadFour = GamePads[3];
    }


    public void Update()
    {        
        for (int i = 0; i < 4; i++)
        {
            GamePadStateArray[i] = GamePad.GetState(PlayerIndexArray[i]);
            if (GamePadStateArray[i].IsConnected && !ControllerConnected[i])
            {
                ControllerConnected[i] = true;
                if (DebugMode)
                    Debug.Log("Controller N°" + (i+1) + " connected.");
            } 
            else if(GamePadStateArray[i].IsConnected && ControllerConnected[i]) 
            {
                UpdateGamePad(i);
            }
            else if (!GamePadStateArray[i].IsConnected && ControllerConnected[i])
            {
                ControllerConnected[i] = false;
                if (DebugMode)
                    Debug.Log("Controller N°" + (i+1) + " disconnected.");
            }
        }
    }

    public void UpdateGamePad(int controllerId)
    {
        GamePads[controllerId].LeftTrigger = GamePadStateArray[controllerId].Triggers.Left;
        GamePads[controllerId].RightTrigger = GamePadStateArray[controllerId].Triggers.Right;

        GamePads[controllerId].LeftStick.X = GamePadStateArray[controllerId].ThumbSticks.Left.X;
        GamePads[controllerId].LeftStick.Y = GamePadStateArray[controllerId].ThumbSticks.Left.Y;
        GamePads[controllerId].RightStick.X = GamePadStateArray[controllerId].ThumbSticks.Right.X;
        GamePads[controllerId].RightStick.Y = GamePadStateArray[controllerId].ThumbSticks.Right.Y;

        UpdateGamePadButtons(controllerId);
    }

    private void UpdateGamePadButtons(int controllerId)
    {
        var curGamePad = GamePadStateArray[controllerId];
        for (int i = 0; i < 14; i++)
        {
            ButtonState curStateButton = ButtonState.Released;
            Controller.ButtonsStates curButton = new Controller.ButtonsStates();

            if (i <= _abxy.Length - 1)
            {
                curButton = (Controller.ButtonsStates)GamePads[controllerId].GetType().GetField(_abxy[i]).GetValue(GamePads[controllerId]); 
                curStateButton = (ButtonState)typeof(GamePadButtons).GetProperty(_abxyXinput[i]).GetValue(GamePadStateArray[controllerId].Buttons, null);
             }
             else
             {
                 int iValue = i - 10;
                curButton = (Controller.ButtonsStates)GamePads[controllerId].GetType().GetField(_dPad[iValue].ToUpper()).GetValue(GamePads[controllerId]);
                curStateButton = (ButtonState)typeof(GamePadDPad).GetProperty(_dPad[iValue]).GetValue(GamePadStateArray[controllerId].DPad, null);
             }
             
            if (curButton.Pressed && curStateButton == ButtonState.Released)
            {
                curButton.Pressed = false;
                //Or use reflection again curButton.GetType().GetField("Pressed").SetValue(curButton, false);
            }
            if (!curButton.Pressed && (curButton.Released || curButton.Held) && curStateButton == ButtonState.Released)
            {
                curButton.Pressed = true;
                curButton.Held = false;
                curButton.Released = false;

            }
            if (!curButton.Pressed && curButton.Released && curStateButton == ButtonState.Pressed)
            {
                curButton.Held = true;
                curButton.Released = false;
            }
            if (!curButton.Pressed && !curButton.Released && !curButton.Held && curStateButton == ButtonState.Pressed)
            {
                curButton.Released = true;
            }             
        }
    }

    public void StopControllerVibration(int controllerId)
    {
        GamePads[controllerId].StopVibration();
    }

    public class Controller
    {
        public ButtonsStates
            A, B, X, Y, LB, RB, start, select, L3, R3, UP, DOWN, LEFT, RIGHT;

        public float
            LeftTrigger, RightTrigger;

        public StickStates
            LeftStick, RightStick;


        private int _controllerId;

        public Controller(int controllerId)
        {
            _controllerId = controllerId;

            LeftStick = new StickStates();
            RightStick = new StickStates();

            A = new ButtonsStates(); 
            B = new ButtonsStates();  
            X = new ButtonsStates();  
            Y = new ButtonsStates();  
            LB = new ButtonsStates(); 
            RB = new ButtonsStates();  
            start = new ButtonsStates();  
            select = new ButtonsStates(); 
            L3 = new ButtonsStates();
            R3 = new ButtonsStates();
            UP = new ButtonsStates();
            DOWN = new ButtonsStates();
            LEFT = new ButtonsStates();
            RIGHT = new ButtonsStates();             
        }

        public void SetVibration(float leftMotor, float rightMotor)
        {
            if (GamePadController.GamePadStateArray[_controllerId].IsConnected)
            {
                PlayerIndex curPlayerIndex = GamePadController.PlayerIndexArray[_controllerId];

                leftMotor /= 100f;
                rightMotor /= 100f;

                GamePad.SetVibration(curPlayerIndex, leftMotor, rightMotor);
            }
        }

        public void SetVibration(float leftMotor, float rightMotor, float timer)
        {
            if (GamePadController.GamePadStateArray[_controllerId].IsConnected)
            {
                PlayerIndex curPlayerIndex = GamePadController.PlayerIndexArray[_controllerId];

                leftMotor /= 100f;
                rightMotor /= 100f;

                GamePad.SetVibration(curPlayerIndex, leftMotor, rightMotor);

                GamePadController.VibrationTimers[_controllerId] = timer;
            }
        }

        public void StopVibration()
        {
            PlayerIndex curPlayerIndex = GamePadController.PlayerIndexArray[_controllerId];
            GamePad.SetVibration(curPlayerIndex, 0, 0);
        }

        public class ButtonsStates
        {
            public bool
                Pressed, Released, Held, Zero = false;
        }

        public class StickStates
        {
            public float
                X, Y = 0f;
        }
    }

    [System.Serializable]
    public class DebugGamePad
    {
        public bool[] ControllersConnected = new bool[] {false, false, false, false};
        public enum DebugForGamePad
        {
            One,
            Two,
            Three,
            Four
        }
        public DebugForGamePad DebugGamePad_N;

        [SerializeField]
        private string Messages = "Debug not enabled.";
        [SerializeField]
        private string CurrentButton = "No input detected.";

        public Vector2 LeftStick, RightStick;
        public float LeftTrigger, RightTrigger;

        public enum ButtonStates
        {
            Released,
            Pressed,
            Held,
            Zero
        }
        public ButtonStates A, B, X, Y, LB, RB, Start, Select, R3, L3, UP, DOWN, LEFT, RIGHT = ButtonStates.Zero;
        private string[] Buttons = new string[] { "A", "B", "X", "Y", "LB", "RB", "Start", "Select", "R3", "L3", "UP", "DOWN", "LEFT", "RIGHT" };

        public void Update()
        {
            ControllersConnected = GamePadController.ControllerConnected;
            if (GamePadController.DebugMode)
            {
                if(DebugGamePad_N == DebugForGamePad.One) {
                    if (ControllersConnected[0])
                    {
                        Messages = "GamePad \"1\" detected.";
                        ShowDebug(0);
                    }
                    else
                        Messages = "GamePad \"1\" not connected.";
                }
                else if (DebugGamePad_N == DebugForGamePad.Two)
                {
                    if (ControllersConnected[1])
                    {
                        Messages = "GamePad \"2\" detected.";
                        ShowDebug(1);
                    }
                    else
                        Messages = "GamePad \"2\" not connected.";
                }
                else if (DebugGamePad_N == DebugForGamePad.Three)
                {
                    if (ControllersConnected[2])
                    {
                        Messages = "GamePad \"3\" detected.";
                        ShowDebug(2);
                    }
                    else
                        Messages = "GamePad \"3\" not connected.";
                }
                else if (DebugGamePad_N == DebugForGamePad.Four)
                {
                    if (ControllersConnected[3])
                    {
                        Messages = "GamePad \"4\" detected.";
                        ShowDebug(3);
                    }
                    else
                        Messages = "GamePad \"4\" not connected.";
                }
                else if (!ControllersConnected[0] && ControllersConnected[1] && ControllersConnected[2] && ControllersConnected[3])
                {
                    Messages = "GamePad is not connected !";
                }
            } else
                Messages = "Debug not enabled.";
        }

        private void ShowDebug(int controllerId){

            LeftTrigger = GamePadController.GamePads[controllerId].LeftTrigger;
            RightTrigger = GamePadController.GamePads[controllerId].RightTrigger;

            LeftStick = new Vector2(GamePadController.GamePads[controllerId].LeftStick.X, GamePadController.GamePads[controllerId].LeftStick.Y);
            RightStick = new Vector2(GamePadController.GamePads[controllerId].RightStick.X, GamePadController.GamePads[controllerId].RightStick.Y);

            for (int i = 0; i < Buttons.Length; i++)
            {
                ButtonStates curButton = (ButtonStates)this.GetType().GetField(Buttons[i]).GetValue(this);
                string gamePadButton = Buttons[i];
                if (gamePadButton == "Start" || gamePadButton == "Select")
                    gamePadButton = gamePadButton.ToLower();
                GamePadController.Controller.ButtonsStates curGamePadButton = (GamePadController.Controller.ButtonsStates)GamePadController.GamePads[controllerId].GetType().GetField(gamePadButton).GetValue(GamePadController.GamePads[controllerId]);

                //this.GetType().GetField(Buttons[i]).SetValue(this, ButtonStates.Released);
                //CurrentButton = "No input detected.";

                /*if (!curGamePadButton.Released && !curGamePadButton.Pressed && !curGamePadButton.Held)
                {
                    curButton = ButtonStates.Zero;
                    this.GetType().GetField(Buttons[i]).SetValue(this, curButton);
                    CurrentButton = "No input detected.";
                }*/

                curButton = ButtonStates.Zero;
                this.GetType().GetField(Buttons[i]).SetValue(this, curButton);

                if (curGamePadButton.Pressed)
                {
                    curButton = ButtonStates.Pressed;
                    this.GetType().GetField(Buttons[i]).SetValue(this, curButton);
                    CurrentButton = Buttons[i] + "";
                }
                if (curGamePadButton.Held)
                {
                    curButton = ButtonStates.Held;
                    this.GetType().GetField(Buttons[i]).SetValue(this, curButton);
                    CurrentButton = Buttons[i] + " Held.";
                }
                if (curGamePadButton.Released)
                {
                    curButton = ButtonStates.Released;
                    this.GetType().GetField(Buttons[i]).SetValue(this, curButton);
                    CurrentButton = Buttons[i] + " Released.";
                }
            }
        }
    }
}
/* Originale options :
Triggers = GamePadStateArray.Triggers.Left, GamePadStateArray.Triggers.Right
DPad = GamePadStateArray.DPad.Up, GamePadStateArray.DPad.Right, GamePadStateArray.DPad.Down, GamePadStateArray.DPad.Left
Start/Select = GamePadStateArray.Buttons.Start, GamePadStateArray.Buttons.Back
L3/R3 = GamePadStateArray.Buttons.LeftStick, GamePadStateArray.Buttons.RightStick
Shoulder Buttons = GamePadStateArray.Buttons.LeftShoulder, GamePadStateArray.Buttons.RightShoulder
ABXY = GamePadStateArray.Buttons.A, GamePadStateArray.Buttons.B, GamePadStateArray.Buttons.X, GamePadStateArray.Buttons.Y);
LeftStick Axis = GamePadStateArray.ThumbSticks.Left.X, GamePadStateArray.ThumbSticks.Left.Y
RightStick Axis = GamePadStateArray.ThumbSticks.Right.X, GamePadStateArray.ThumbSticks.Right.Y
Vibrations (function) = GamePad.SetVibration(playerIndex, LeftMotor, RightMotor) -> any value between 0-65535 
*/