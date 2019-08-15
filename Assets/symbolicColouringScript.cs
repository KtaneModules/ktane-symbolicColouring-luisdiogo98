using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;
using System.Text.RegularExpressions;

public class symbolicColouringScript : MonoBehaviour 
{
	readonly int WHITE = 0;
	readonly int RED = 100;
	readonly int YELLOW = 10;
	readonly int BLUE = 1;
	readonly int ORANGE = 110;
	readonly int PURPLE = 101;
	readonly int GREEN = 11;
	readonly int BLACK = 111;

	public KMBombInfo bomb;
	public KMAudio Audio;

	//Logging
	static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

	public KMSelectable[] symbolBtns;
	public KMSelectable[] colorBtns;

	List<int> symbols = new List<int>();
	List<SymbolInfo> symbolsInfo = new List<SymbolInfo>();
	int color;
	int[] btnOrder;
	int[] btnColors;
	List<int> pressed = new List<int>();
	int nextPress = 0;

	void Awake()
	{
		moduleId = moduleIdCounter++;
		symbolBtns[0].OnInteract += delegate () { HandlePress(0); return false; };
		symbolBtns[1].OnInteract += delegate () { HandlePress(1); return false; };
		symbolBtns[2].OnInteract += delegate () { HandlePress(2); return false; };
		symbolBtns[3].OnInteract += delegate () { HandlePress(3); return false; };
		colorBtns[0].OnInteract += delegate () { HandleWhite(); return false; };
		colorBtns[1].OnInteract += delegate () { HandleRed(); return false; };
		colorBtns[2].OnInteract += delegate () { HandleYellow(); return false; };
		colorBtns[3].OnInteract += delegate () { HandleBlue(); return false; };

	}

	void Start () 
	{
		RandomizeButtons();
		CalcButtonOrder();
		CalcButtonColors();
	}

	void RandomizeButtons()
	{
		for(int i = 0; i < 4; i++)
		{
			int symbol = rnd.Range(0, 52);
			while(Array.Exists<int>(symbols.ToArray(), x => x == symbol))
			{
				symbol = rnd.Range(0, 52);
			}
			symbols.Add(symbol);
			SymbolInfo info = GetSymbolInfo(symbol);
			SetUpBtn(info, i);
			symbolsInfo.Add(info);

			Debug.LogFormat("[Symbolic Colouring #{0}] Button {1} is {2}.", moduleId, i + 1, info.symbol + "");
		}
	}

	void CalcButtonOrder()
	{
		if(bomb.GetSerialNumberNumbers().ElementAt(bomb.GetSerialNumberNumbers().Count() - 1) % 2 != 0)
		{
			switch(bomb.GetBatteryCount())
			{
				case 0: btnOrder = new int[] {2, 0, 1, 3}; break;
				case 1: btnOrder = new int[] {0, 2, 1, 3}; break;
				case 2: btnOrder = new int[] {1, 3, 2, 0}; break;
				case 3: btnOrder = new int[] {2, 3, 1, 0}; break;
				case 4: btnOrder = new int[] {1, 0, 3, 2}; break;
				case 5: btnOrder = new int[] {0, 3, 2, 1}; break;
				case 6: btnOrder = new int[] {3, 1, 0, 2}; break;
				case 7: btnOrder = new int[] {0, 2, 3, 1}; break;
				case 8: btnOrder = new int[] {2, 1, 0, 3}; break;
				case 9: btnOrder = new int[] {1, 2, 0, 3}; break;
				default: btnOrder = new int[] {3, 2, 1, 0}; break;
			}
		}
		else
		{
			switch(bomb.GetBatteryCount())
			{
				case 0: btnOrder = new int[] {2, 3, 0, 1}; break;
				case 1: btnOrder = new int[] {1, 0, 2, 3}; break;
				case 2: btnOrder = new int[] {0, 3, 1, 2}; break;
				case 3: btnOrder = new int[] {2, 0, 3, 1}; break;
				case 4: btnOrder = new int[] {3, 0, 1, 2}; break;
				case 5: btnOrder = new int[] {1, 2, 3, 0}; break;
				case 6: btnOrder = new int[] {0, 1, 3, 2}; break;
				case 7: btnOrder = new int[] {3, 1, 2, 0}; break;
				case 8: btnOrder = new int[] {1, 3, 0, 2}; break;
				case 9: btnOrder = new int[] {2, 1, 3, 0}; break;
				default: btnOrder = new int[] {0, 1, 2, 3}; break;
			}
		}
		
		String log = "";

		foreach (int i in btnOrder)
		{
			log += (i + 1) + " ";
		}

		Debug.LogFormat("[Symbolic Colouring #{0}] Button press order is [ {1}].", moduleId, log);

	}

	void CalcButtonColors()
	{
		btnColors = new int[4];

		for(int i = 0; i < 4; i++)
		{
			btnColors[i] = symbolsInfo.ElementAt(btnOrder[i]).colors[btnOrder[i]];
			Debug.LogFormat("[Symbolic Colouring #{0}] Button {1} must be coloured {2}.", moduleId, btnOrder[i] + 1, GetColorName(btnColors[i]));
		}	
	}

	SymbolInfo GetSymbolInfo(int n)
	{
		switch(n)
		{
			case 0: return new SymbolInfo('α', 18f, -0.11f, new int[] {RED, WHITE, BLACK, GREEN});
			case 1: return new SymbolInfo('β', 14f, -0.04f, new int[] {BLUE, YELLOW, ORANGE, PURPLE});
			case 2: return new SymbolInfo('χ', 18f, -0.33f, new int[] {BLACK, BLUE, GREEN, YELLOW});
			case 3: return new SymbolInfo('δ', 18f, 0.07f, new int[] {BLACK, YELLOW, RED, WHITE});
			case 4: return new SymbolInfo('ε', 21f, -0.18f, new int[] {RED, PURPLE, BLUE, YELLOW});
			case 5: return new SymbolInfo('ϕ', 16f, -0.07f, new int[] {GREEN, ORANGE, RED, BLACK});
			case 6: return new SymbolInfo('γ', 18f, -0.29f, new int[] {YELLOW, RED, WHITE, ORANGE});
			case 7: return new SymbolInfo('η', 18f, -0.29f, new int[] {ORANGE, WHITE, PURPLE, RED});
			case 8: return new SymbolInfo('ι', 21f, -0.17f, new int[] {GREEN, RED, YELLOW, BLUE});
			case 9: return new SymbolInfo('φ', 18f, -0.26f, new int[] {WHITE, GREEN, BLUE, PURPLE});

			case 10: return new SymbolInfo('κ', 20f, -0.11f, new int[] {YELLOW, PURPLE, WHITE, ORANGE});
			case 11: return new SymbolInfo('λ', 18f, 0.08f, new int[] {BLACK, GREEN, RED, BLUE});
			case 12: return new SymbolInfo('μ', 18f, -0.27f, new int[] {ORANGE, BLUE, GREEN, PURPLE});
			case 13: return new SymbolInfo('ν', 20f, -0.14f, new int[] {BLUE, BLACK, YELLOW, GREEN});
			case 14: return new SymbolInfo('π', 20f, -0.14f, new int[] {PURPLE, WHITE, BLUE, GREEN});
			case 15: return new SymbolInfo('θ', 18f, 0.02f, new int[] {GREEN, RED, PURPLE, BLACK});
			case 16: return new SymbolInfo('ρ', 18f, -0.36f, new int[] {BLUE, GREEN, RED, YELLOW});
			case 17: return new SymbolInfo('σ', 20f, -0.14f, new int[] {GREEN, ORANGE, ORANGE, WHITE});
			case 18: return new SymbolInfo('τ', 20f, -0.17f, new int[] {PURPLE, YELLOW, GREEN, RED});
			case 19: return new SymbolInfo('υ', 20f, -0.17f, new int[] {RED, ORANGE, WHITE, GREEN});

			case 20: return new SymbolInfo('ϖ', 18f, -0.17f, new int[] {PURPLE, BLUE, YELLOW, BLACK});
			case 21: return new SymbolInfo('ω', 18f, -0.17f, new int[] {ORANGE, RED, BLACK, ORANGE});
			case 22: return new SymbolInfo('ξ', 15f, -0.11f, new int[] {RED, WHITE, ORANGE, GREEN});
			case 23: return new SymbolInfo('ψ', 18f, -0.33f, new int[] {WHITE, PURPLE, RED, RED});
			case 24: return new SymbolInfo('ζ', 15.4f, -0.07f, new int[] {GREEN, BLACK, YELLOW, ORANGE});
			case 25: return new SymbolInfo('⌠', 13f, 0.01f, new int[] {YELLOW, RED, PURPLE, WHITE});
			case 26: return new SymbolInfo('ℑ', 15f, 0.11f, new int[] {BLUE, GREEN, ORANGE, BLACK});
			case 27: return new SymbolInfo('ℜ', 15f, 0.11f, new int[] {YELLOW, PURPLE, YELLOW, ORANGE});
			case 28: return new SymbolInfo('℘', 21f, -0.05f, new int[] {PURPLE, ORANGE, BLUE, GREEN});
			case 29: return new SymbolInfo('⊥', 18f, 0.04f, new int[] {ORANGE, RED, YELLOW, RED});
		
			case 30: return new SymbolInfo('∆', 18f, 0.04f, new int[] {RED, WHITE, PURPLE, BLUE});
			case 31: return new SymbolInfo('Φ', 18f, 0.04f, new int[] {WHITE, BLUE, GREEN, RED});
			case 32: return new SymbolInfo('Γ', 18f, 0.04f, new int[] {RED, ORANGE, PURPLE, PURPLE});
			case 33: return new SymbolInfo('ϑ', 18f, 0.04f, new int[] {GREEN, BLACK, YELLOW, WHITE});
			case 34: return new SymbolInfo('Λ', 18f, 0.04f, new int[] {RED, YELLOW, WHITE, BLUE});
			case 35: return new SymbolInfo('Π', 18f, 0.04f, new int[] {ORANGE, WHITE, YELLOW, GREEN});
			case 36: return new SymbolInfo('Θ', 18f, 0.04f, new int[] {GREEN, RED, BLACK, ORANGE});
			case 37: return new SymbolInfo('Σ', 18f, 0.04f, new int[] {BLACK, YELLOW, ORANGE, RED});
			case 38: return new SymbolInfo('ς', 18f, -0.32f, new int[] {RED, PURPLE, RED, BLUE});
			case 39: return new SymbolInfo('Ω', 18f, 0.04f, new int[] {GREEN, ORANGE, BLUE, BLACK});
		
			case 40: return new SymbolInfo('Ξ', 18f, 0.04f, new int[] {PURPLE, YELLOW, PURPLE, WHITE});
			case 41: return new SymbolInfo('Ψ', 18f, 0.04f, new int[] {WHITE, BLACK, GREEN, RED});
			case 42: return new SymbolInfo('⊕', 18f, -0.09f, new int[] {YELLOW, ORANGE, BLUE, GREEN});
			case 43: return new SymbolInfo('ฯ', 18f, -0.07f, new int[] {PURPLE, GREEN, RED, YELLOW});
			case 44: return new SymbolInfo('∋', 18f, 0f, new int[] {BLACK, RED, WHITE, ORANGE});
			case 45: return new SymbolInfo('∀', 18f, 0.04f, new int[] {RED, WHITE, PURPLE, GREEN});
			case 46: return new SymbolInfo('ϒ', 18f, 0.04f, new int[] {WHITE, GREEN, YELLOW, WHITE});
			case 47: return new SymbolInfo('∞', 18f, -0.07f, new int[] {PURPLE, BLACK, BLUE, RED});
			case 48: return new SymbolInfo('ƒ', 16f, -0.11f, new int[] {ORANGE, BLUE, YELLOW, GREEN});
			case 49: return new SymbolInfo('א', 18f, -0.07f, new int[] {BLACK, PURPLE, BLUE, WHITE});
		
			case 50: return new SymbolInfo('∝', 18f, 0f, new int[] {YELLOW, RED, WHITE, BLACK});
			case 51: return new SymbolInfo('∅', 21f, 0.11f, new int[] {BLUE, GREEN, RED, ORANGE});
			
		}

		return null;
	}

	void SetUpBtn(SymbolInfo info, int n)
	{
		symbolBtns[n].gameObject.transform.Find("symbol").gameObject.GetComponentInChildren<TextMesh>().text = info.symbol + "";
		Vector3 pos = symbolBtns[n].gameObject.transform.Find("symbol").localPosition;
		pos.y = info.yPos - 0.03f;
		pos.x += 0.02f;
		symbolBtns[n].gameObject.transform.Find("symbol").localPosition = pos;
		symbolBtns[n].gameObject.transform.Find("symbol").GetComponentInChildren<TextMesh>().characterSize = info.charSize;
	}

	void HandleWhite()
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		colorBtns[0].AddInteractionPunch(.5f);
		
		if(moduleSolved)
			return;

		color = 0;
		Debug.LogFormat("[Symbolic Colouring #{0}] Pressed the White button. Color is now {1}.", moduleId, GetColorName(color));
	}

	void HandleRed()
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		colorBtns[1].AddInteractionPunch(.5f);
		
		if(moduleSolved)
			return;

		if(color < 100) color += 100;
		Debug.LogFormat("[Symbolic Colouring #{0}] Pressed the Red button. Color is now {1}.", moduleId, GetColorName(color));
	}

	void HandleYellow()
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		colorBtns[2].AddInteractionPunch(.5f);
		
		if(moduleSolved)
			return;

		if( (color % 100) < 10) color += 10;
		Debug.LogFormat("[Symbolic Colouring #{0}] Pressed the Yellow button. Color is now {1}.", moduleId, GetColorName(color));
	}

	void HandleBlue()
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		colorBtns[3].AddInteractionPunch(.5f);
		
		if(moduleSolved)
			return;

		if( (color % 2) == 0) color += 1;
		Debug.LogFormat("[Symbolic Colouring #{0}] Pressed the Blue button. Color is now {1}.", moduleId, GetColorName(color));
	}

	void HandlePress(int i)
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		symbolBtns[i].AddInteractionPunch(.5f);

		if(moduleSolved || Array.Exists<int>(pressed.ToArray(), x => x == i))
			return;

		if(btnOrder[nextPress] != i)
		{
			Debug.LogFormat("[Symbolic Colouring #{0}] Strike! Pressed button {1}. Expected button {2}.", moduleId, i+1, btnOrder[nextPress] + 1);
			GetComponent<KMBombModule>().HandleStrike();
			return;
		}

		if(color != btnColors[nextPress])
		{
			Debug.LogFormat("[Symbolic Colouring #{0}] Strike! Colored button {1} with {2}. Expected {3}.", moduleId, i + 1, GetColorName(color), GetColorName(btnColors[nextPress]));
			GetComponent<KMBombModule>().HandleStrike();
			return;
		}

		pressed.Add(i);
		nextPress++;

		if(nextPress == 4)
		{
			moduleSolved = true;
			Debug.LogFormat("[Symbolic Colouring #{0}] Successfuly colored button {1} with {2}. Module solved.", moduleId, i + 1, GetColorName(color));
			GetComponent<KMBombModule>().HandlePass();
		}
		else
		{
			Debug.LogFormat("[Symbolic Colouring #{0}] Successfuly colored button {1} with {2}.", moduleId, i + 1, GetColorName(color));
		}
	}

	String GetColorName(int c)
	{
		switch(c)
		{
			case 0: return "White";
			case 100: return "Red";
			case 10: return "Yellow";
			case 1: return "Blue";
			case 110: return "Orange";
			case 101: return "Purple";
			case 11: return "Green";
			case 111: return "Black";
		}

		return "";
	}

    //twitch plays
    private bool cmdIsValid(string param)
    {
        string[] parameters = param.Split(' ', ',');
        for (int i = 1; i < parameters.Length; i++)
        {
            if (!parameters[i].EqualsIgnoreCase("1") && !parameters[i].EqualsIgnoreCase("2") && !parameters[i].EqualsIgnoreCase("3") && !parameters[i].EqualsIgnoreCase("4") && !parameters[i].EqualsIgnoreCase("red") && !parameters[i].EqualsIgnoreCase("white") && !parameters[i].EqualsIgnoreCase("blue") && !parameters[i].EqualsIgnoreCase("yellow"))
            {
                return false;
            }
        }
        return true;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <button> [Presses the specified button] | !{0} press <button> <button> [Example of button chaining] | !{0} reset [Resets all inputs] | Valid buttons are white, red, yellow, blue, and 1-4 being the symbol buttons from left to right";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*reset\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            Debug.LogFormat("[Symbolic Colouring #{0}] Reset of inputs triggered! (TP)", moduleId);
            colorBtns[0].OnInteract();
            pressed = new List<int>();
            nextPress = 0;
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length > 1)
            {
                if (cmdIsValid(command))
                {
                    yield return null;
                    for (int i = 1; i < parameters.Length; i++)
                    {
                        if (parameters[i].EqualsIgnoreCase("1"))
                        {
                            symbolBtns[0].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("2"))
                        {
                            symbolBtns[1].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("3"))
                        {
                            symbolBtns[2].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("4"))
                        {
                            symbolBtns[3].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("white"))
                        {
                            colorBtns[0].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("red"))
                        {
                            colorBtns[1].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("yellow"))
                        {
                            colorBtns[2].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("blue"))
                        {
                            colorBtns[3].OnInteract();
                        }
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            yield break;
        }
    }
}
