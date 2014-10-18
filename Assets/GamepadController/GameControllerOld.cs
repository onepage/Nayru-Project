using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure; // Required in C#

public class GameController : MonoBehaviour{

	public static bool[] controllerConnected = new bool[4];
	public  static GamePadState[] GamePadStateArray = new GamePadState[4];
	public PlayerIndex[] playerIndexArray = new PlayerIndex[4];
	public static List<Dictionary<string, ButtonsStates>> buttonStates = new List<Dictionary<string, ButtonsStates>>();


	// Use this for initialization
	void Start () {
		playerIndexArray[0] = PlayerIndex.One;
		playerIndexArray[1] = PlayerIndex.Two;
		playerIndexArray[2] = PlayerIndex.Three;
		playerIndexArray[3] = PlayerIndex.Four;
		
		for(int i= 0; i < 4; i++){
			GamePadStateArray[i] = GamePad.GetState(playerIndexArray[i]);	
			buttonStates.Add(new Dictionary<string, ButtonsStates>());
			Dictionary<string, ButtonsStates> curController = buttonStates[i];
			curController["A"] = new ButtonsStates();
			curController["B"] = new ButtonsStates();
			curController["X"] = new ButtonsStates();
			curController["Y"] = new ButtonsStates();
			curController["Up"] = new ButtonsStates();
			curController["Down"] = new ButtonsStates();
			curController["Left"] = new ButtonsStates();
			curController["Right"] = new ButtonsStates();
			curController["Left"] = new ButtonsStates();
			curController["Start"] = new ButtonsStates();
			curController["Back"] = new ButtonsStates();
			curController["LB"] = new ButtonsStates();
			curController["RB"] = new ButtonsStates();
			curController["L3"] = new ButtonsStates();
			curController["R3"] = new ButtonsStates();

			buttonStates[i] = curController;
		}
		/* Available options :
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
	}
	
	// Update is called once per frame
	void Update () {		
		for(int i= 0; i < 4; i++){
			GamePadStateArray[i] = GamePad.GetState(playerIndexArray[i]);			
			//Debug.Log("GC = "+GamePadStateArray[0].Buttons.LeftShoulder);
			if (GamePadStateArray[i].IsConnected) {
				controllerConnected[i] = true;
				CheckButtonPress(GamePadStateArray[i], i);
			} else
				controllerConnected[i] = false;
			/*if(controllerConnected[i])
				Debug.Log("Controller"+i+" connected !");*/
		}
	}

	void CheckButtonPress(GamePadState curGamePad, int gamePadId){
		Dictionary<string, ButtonsStates> curButtonDic = buttonStates[gamePadId];
		/*
		curButtonDic["UP"].buttonUp
		curButtonDic["UP"].buttonDown
		curButtonDic["UP"].buttonHeld
		*/
		
		//button UP
		if(curButtonDic["Up"].buttonUp && curGamePad.DPad.Up == ButtonState.Released) {
			curButtonDic["Up"].buttonUp = false;
		}
		if(!curButtonDic["Up"].buttonUp && (curButtonDic["Up"].buttonDown || curButtonDic["Up"].buttonHeld) && curGamePad.DPad.Up == ButtonState.Released) {
			curButtonDic["Up"].buttonUp = true;
			curButtonDic["Up"].buttonHeld = false;
		}
		if(!curButtonDic["Up"].buttonUp && curButtonDic["Up"].buttonDown && curGamePad.DPad.Up == ButtonState.Pressed) {
			curButtonDic["Up"].buttonHeld = true;				
			curButtonDic["Up"].buttonDown = false;
		}
		if(!curButtonDic["Up"].buttonUp && !curButtonDic["Up"].buttonDown && !curButtonDic["Up"].buttonHeld && curGamePad.DPad.Up == ButtonState.Pressed) {
			curButtonDic["Up"].buttonDown = true;
		}
		
		//button Down
		if(curButtonDic["Down"].buttonUp && curGamePad.DPad.Down == ButtonState.Released) {
			curButtonDic["Down"].buttonUp = false;
		}
		if(!curButtonDic["Down"].buttonUp && (curButtonDic["Down"].buttonDown || curButtonDic["Down"].buttonHeld) && curGamePad.DPad.Down == ButtonState.Released) {
			curButtonDic["Down"].buttonUp = true;
			curButtonDic["Down"].buttonHeld = false;
		}
		if(!curButtonDic["Down"].buttonUp && curButtonDic["Down"].buttonDown && curGamePad.DPad.Down == ButtonState.Pressed) {
			curButtonDic["Down"].buttonHeld = true;				
			curButtonDic["Down"].buttonDown = false;
		}
		if(!curButtonDic["Down"].buttonUp && !curButtonDic["Down"].buttonDown && !curButtonDic["Down"].buttonHeld && curGamePad.DPad.Down == ButtonState.Pressed) {
			curButtonDic["Down"].buttonDown = true;
		}
		
		//button Right
		if(curButtonDic["Right"].buttonUp && curGamePad.DPad.Right == ButtonState.Released) {
			curButtonDic["Right"].buttonUp = false;
		}
		if(!curButtonDic["Right"].buttonUp && (curButtonDic["Right"].buttonDown || curButtonDic["Right"].buttonHeld) && curGamePad.DPad.Right == ButtonState.Released) {
			curButtonDic["Right"].buttonUp = true;
			curButtonDic["Right"].buttonHeld = false;
		}
		if(!curButtonDic["Right"].buttonUp && curButtonDic["Right"].buttonDown && curGamePad.DPad.Right == ButtonState.Pressed) {
			curButtonDic["Right"].buttonHeld = true;				
			curButtonDic["Right"].buttonDown = false;
		}
		if(!curButtonDic["Right"].buttonUp && !curButtonDic["Right"].buttonDown && !curButtonDic["Right"].buttonHeld && curGamePad.DPad.Right == ButtonState.Pressed) {
			curButtonDic["Right"].buttonDown = true;
		}
		
		//button Left
		if(curButtonDic["Left"].buttonUp && curGamePad.DPad.Left == ButtonState.Released) {
			curButtonDic["Left"].buttonUp = false;
		}
		if(!curButtonDic["Left"].buttonUp && (curButtonDic["Left"].buttonDown || curButtonDic["Left"].buttonHeld) && curGamePad.DPad.Left == ButtonState.Released) {
			curButtonDic["Left"].buttonUp = true;
			curButtonDic["Left"].buttonHeld = false;
		}
		if(!curButtonDic["Left"].buttonUp && curButtonDic["Left"].buttonDown && curGamePad.DPad.Left == ButtonState.Pressed) {
			curButtonDic["Left"].buttonHeld = true;				
			curButtonDic["Left"].buttonDown = false;
		}
		if(!curButtonDic["Left"].buttonUp && !curButtonDic["Left"].buttonDown && !curButtonDic["Left"].buttonHeld && curGamePad.DPad.Left == ButtonState.Pressed) {
			curButtonDic["Left"].buttonDown = true;
		}
		
		//button A
		if(curButtonDic["A"].buttonUp && curGamePad.Buttons.A == ButtonState.Released) {
			curButtonDic["A"].buttonUp = false;
		}
		if(!curButtonDic["A"].buttonUp && (curButtonDic["A"].buttonDown || curButtonDic["A"].buttonHeld) && curGamePad.Buttons.A == ButtonState.Released) {
			curButtonDic["A"].buttonUp = true;
			curButtonDic["A"].buttonHeld = false;
		}
		if(!curButtonDic["A"].buttonUp && curButtonDic["A"].buttonDown && curGamePad.Buttons.A == ButtonState.Pressed) {
			curButtonDic["A"].buttonHeld = true;				
			curButtonDic["A"].buttonDown = false;
		}
		if(!curButtonDic["A"].buttonUp && !curButtonDic["A"].buttonDown && !curButtonDic["A"].buttonHeld && curGamePad.Buttons.A == ButtonState.Pressed) {
			curButtonDic["A"].buttonDown = true;
		}
		
		//button B
		if(curButtonDic["B"].buttonUp && curGamePad.Buttons.B == ButtonState.Released) {
			curButtonDic["B"].buttonUp = false;
		}
		if(!curButtonDic["B"].buttonUp && (curButtonDic["B"].buttonDown || curButtonDic["B"].buttonHeld) && curGamePad.Buttons.B == ButtonState.Released) {
			curButtonDic["B"].buttonUp = true;
			curButtonDic["B"].buttonHeld = false;
		}
		if(!curButtonDic["B"].buttonUp && curButtonDic["B"].buttonDown && curGamePad.Buttons.B == ButtonState.Pressed) {
			curButtonDic["B"].buttonHeld = true;				
			curButtonDic["B"].buttonDown = false;
		}
		if(!curButtonDic["B"].buttonUp && !curButtonDic["B"].buttonDown && !curButtonDic["B"].buttonHeld && curGamePad.Buttons.B == ButtonState.Pressed) {
			curButtonDic["B"].buttonDown = true;
		}
		
		//button X
		if(curButtonDic["X"].buttonUp && curGamePad.Buttons.X == ButtonState.Released) {
			curButtonDic["X"].buttonUp = false;
		}
		if(!curButtonDic["X"].buttonUp && (curButtonDic["X"].buttonDown || curButtonDic["X"].buttonHeld) && curGamePad.Buttons.X == ButtonState.Released) {
			curButtonDic["X"].buttonUp = true;
			curButtonDic["X"].buttonHeld = false;
		}
		if(!curButtonDic["X"].buttonUp && curButtonDic["X"].buttonDown && curGamePad.Buttons.X == ButtonState.Pressed) {
			curButtonDic["X"].buttonHeld = true;				
			curButtonDic["X"].buttonDown = false;
		}
		if(!curButtonDic["X"].buttonUp && !curButtonDic["X"].buttonDown && !curButtonDic["X"].buttonHeld && curGamePad.Buttons.X == ButtonState.Pressed) {
			curButtonDic["X"].buttonDown = true;
		}
		
		//button Y
		if(curButtonDic["Y"].buttonUp && curGamePad.Buttons.Y == ButtonState.Released) {
			curButtonDic["Y"].buttonUp = false;
		}
		if(!curButtonDic["Y"].buttonUp && (curButtonDic["Y"].buttonDown || curButtonDic["Y"].buttonHeld) && curGamePad.Buttons.Y == ButtonState.Released) {
			curButtonDic["Y"].buttonUp = true;
			curButtonDic["Y"].buttonHeld = false;
		}
		if(!curButtonDic["Y"].buttonUp && curButtonDic["Y"].buttonDown && curGamePad.Buttons.Y == ButtonState.Pressed) {
			curButtonDic["Y"].buttonHeld = true;				
			curButtonDic["Y"].buttonDown = false;
		}
		if(!curButtonDic["Y"].buttonUp && !curButtonDic["Y"].buttonDown && !curButtonDic["Y"].buttonHeld && curGamePad.Buttons.Y == ButtonState.Pressed) {
			curButtonDic["Y"].buttonDown = true;
		}
		
		//button Start
		if(curButtonDic["Start"].buttonUp && curGamePad.Buttons.Start == ButtonState.Released) {
			curButtonDic["Start"].buttonUp = false;
		}
		if(!curButtonDic["Start"].buttonUp && (curButtonDic["Start"].buttonDown || curButtonDic["Start"].buttonHeld) && curGamePad.Buttons.Start == ButtonState.Released) {
			curButtonDic["Start"].buttonUp = true;
			curButtonDic["Start"].buttonHeld = false;
		}
		if(!curButtonDic["Start"].buttonUp && curButtonDic["Start"].buttonDown && curGamePad.Buttons.Start == ButtonState.Pressed) {
			curButtonDic["Start"].buttonHeld = true;				
			curButtonDic["Start"].buttonDown = false;
		}
		if(!curButtonDic["Start"].buttonUp && !curButtonDic["Start"].buttonDown && !curButtonDic["Start"].buttonHeld && curGamePad.Buttons.Start == ButtonState.Pressed) {
			curButtonDic["Start"].buttonDown = true;
		}
		
		//button Back
		if(curButtonDic["Back"].buttonUp && curGamePad.Buttons.Back == ButtonState.Released) {
			curButtonDic["Back"].buttonUp = false;
		}
		if(!curButtonDic["Back"].buttonUp && (curButtonDic["Back"].buttonDown || curButtonDic["Back"].buttonHeld) && curGamePad.Buttons.Back == ButtonState.Released) {
			curButtonDic["Back"].buttonUp = true;
			curButtonDic["Back"].buttonHeld = false;
		}
		if(!curButtonDic["Back"].buttonUp && curButtonDic["Back"].buttonDown && curGamePad.Buttons.Back == ButtonState.Pressed) {
			curButtonDic["Back"].buttonHeld = true;				
			curButtonDic["Back"].buttonDown = false;
		}
		if(!curButtonDic["Back"].buttonUp && !curButtonDic["Back"].buttonDown && !curButtonDic["Back"].buttonHeld && curGamePad.Buttons.Back == ButtonState.Pressed) {
			curButtonDic["Back"].buttonDown = true;
		}
		
		//button LeftShoulder
		if(curButtonDic["LB"].buttonUp && curGamePad.Buttons.LeftShoulder == ButtonState.Released) {
			curButtonDic["LB"].buttonUp = false;
		}
		if(!curButtonDic["LB"].buttonUp && (curButtonDic["LB"].buttonDown || curButtonDic["LB"].buttonHeld) && curGamePad.Buttons.LeftShoulder == ButtonState.Released) {
			curButtonDic["LB"].buttonUp = true;
			curButtonDic["LB"].buttonHeld = false;
		}
		if(!curButtonDic["LB"].buttonUp && curButtonDic["LB"].buttonDown && curGamePad.Buttons.LeftShoulder == ButtonState.Pressed) {
			curButtonDic["LB"].buttonHeld = true;				
			curButtonDic["LB"].buttonDown = false;
		}
		if(!curButtonDic["LB"].buttonUp && !curButtonDic["LB"].buttonDown && !curButtonDic["LB"].buttonHeld && curGamePad.Buttons.LeftShoulder == ButtonState.Pressed) {
			curButtonDic["LB"].buttonDown = true;
		}
		
		//button RightShoulder
		if(curButtonDic["RB"].buttonUp && curGamePad.Buttons.RightShoulder == ButtonState.Released) {
			curButtonDic["RB"].buttonUp = false;
		}
		if(!curButtonDic["RB"].buttonUp && (curButtonDic["RB"].buttonDown || curButtonDic["RB"].buttonHeld) && curGamePad.Buttons.RightShoulder == ButtonState.Released) {
			curButtonDic["RB"].buttonUp = true;
			curButtonDic["RB"].buttonHeld = false;
		}
		if(!curButtonDic["RB"].buttonUp && curButtonDic["RB"].buttonDown && curGamePad.Buttons.RightShoulder == ButtonState.Pressed) {
			curButtonDic["RB"].buttonHeld = true;				
			curButtonDic["RB"].buttonDown = false;
		}
		if(!curButtonDic["RB"].buttonUp && !curButtonDic["RB"].buttonDown && !curButtonDic["RB"].buttonHeld && curGamePad.Buttons.RightShoulder == ButtonState.Pressed) {
			curButtonDic["RB"].buttonDown = true;
		}
		
		//button L3
		if(curButtonDic["L3"].buttonUp && curGamePad.Buttons.LeftStick == ButtonState.Released) {
			curButtonDic["L3"].buttonUp = false;
		}
		if(!curButtonDic["L3"].buttonUp && (curButtonDic["L3"].buttonDown || curButtonDic["L3"].buttonHeld) && curGamePad.Buttons.LeftStick == ButtonState.Released) {
			curButtonDic["L3"].buttonUp = true;
			curButtonDic["L3"].buttonHeld = false;
		}
		if(!curButtonDic["L3"].buttonUp && curButtonDic["L3"].buttonDown && curGamePad.Buttons.LeftStick == ButtonState.Pressed) {
			curButtonDic["L3"].buttonHeld = true;				
			curButtonDic["L3"].buttonDown = false;
		}
		if(!curButtonDic["L3"].buttonUp && !curButtonDic["L3"].buttonDown && !curButtonDic["L3"].buttonHeld && curGamePad.Buttons.LeftStick == ButtonState.Pressed) {
			curButtonDic["L3"].buttonDown = true;
		}
		
		//button R3
		if(curButtonDic["R3"].buttonUp && curGamePad.Buttons.RightStick == ButtonState.Released) {
			curButtonDic["R3"].buttonUp = false;
		}
		if(!curButtonDic["R3"].buttonUp && (curButtonDic["R3"].buttonDown || curButtonDic["R3"].buttonHeld) && curGamePad.Buttons.RightStick == ButtonState.Released) {
			curButtonDic["R3"].buttonUp = true;
			curButtonDic["R3"].buttonHeld = false;
		}
		if(!curButtonDic["R3"].buttonUp && curButtonDic["R3"].buttonDown && curGamePad.Buttons.RightStick == ButtonState.Pressed) {
			curButtonDic["R3"].buttonHeld = true;				
			curButtonDic["R3"].buttonDown = false;
		}
		if(!curButtonDic["R3"].buttonUp && !curButtonDic["R3"].buttonDown && !curButtonDic["R3"].buttonHeld && curGamePad.Buttons.RightStick == ButtonState.Pressed) {
			curButtonDic["R3"].buttonDown = true;
		}
	}
}

public class ButtonsStates {
	public bool buttonUp = false;
	public bool buttonDown = false;
	public bool buttonHeld = false;
}