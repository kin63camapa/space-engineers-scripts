string displaysPrefix = "Bat info"; 
string reactorsPrefix = "Аварийный реактор"; 
double percentOfDischange = 0.5; 
double percentOfСhange = 30; 
 
public Program() 
{ 
	    List<IMyTimerBlock> timers = new List<IMyTimerBlock>(); 
    GridTerminalSystem.GetBlocksOfType<IMyTimerBlock>(timers); 
    for(int i = 0; i < timers.Count; i++) 
    { 
        timers[i].ApplyAction("TriggerNow"); 
    } 
} 
 
void setScreensText(string text,Color color) 
{ 
    List<IMyTerminalBlock> screens = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(screens); 
    for(int i = 0; i < screens.Count; i++) 
    { 
        IMyTextPanel screen = (IMyTextPanel) screens[i]; 
        if(screen.CustomName.IndexOf(displaysPrefix) == 0) 
        { 
            screen.WritePublicText(text); 
            screen.WritePrivateText(text); 
            screen.SetValue("FontColor", color); 
        } 
    } 
} 
 
void reactorsPower(bool on) 
{ 
    List<IMyReactor> reactors = new List<IMyReactor>(); 
    GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactors); 
    for(int i = 0; i < reactors.Count; i++) 
    { 
        if(reactors[i].CustomName.IndexOf(reactorsPrefix) == 0) 
        {
            if (on) reactors[i].ApplyAction("OnOff_On"); 
            else reactors[i].ApplyAction("OnOff_Off"); 
        } 
    } 
} 
 
 
void blocksPower(bool on) 
{ 
	List<IMyTerminalBlock> ligts = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(ligts); 
    for(int i = 0; i < ligts.Count; i++) 
    { ;
	        //if (on) ligts[i].ApplyAction("OnOff_On"); 
        //else ligts[i].ApplyAction("OnOff_Off"); 
    } 
} 
 
void Main() 
{ 
	int batteriesCount=0; 
    double change=0; 
    double percent=0; 
    string reactor = "Reactors OFF!"; 
    Color color = Color.Green; 
    List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>(); 
    GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries); 
    for(int i = 0; i < batteries.Count; i++) 
    { 
        IMyBatteryBlock battery = (IMyBatteryBlock) batteries[i]; 
        if(battery.HasCapacityRemaining && battery.IsFunctional && battery.IsWorking) 
		{ 
			change += battery.CurrentStoredPower; 
			batteriesCount++; 
		} 
    } 
    change = change/batteriesCount; 
    percent = (change/3)*100; 
    if (percent < percentOfDischange || batteries.Count == 0 ) 
    { 
        color = Color.Red; 
        blocksPower(false); 
        reactorsPower(true); 
        reactor = "Reactors ON!"; 
    } 
    else 
    { 
		if (percent < percentOfСhange) 
		{ 
			color = Color.Orange; 
		} 
		else 
		{ 
			reactorsPower(false); 
			blocksPower(true); 
		} 
    } 
    change = (Convert.ToDouble(Convert.ToInt32(change*1000))/1000)*batteriesCount; 
    percent = Convert.ToDouble(Convert.ToInt32(percent*1000))/1000; 
    setScreensText("Batteries "+Convert.ToString(batteriesCount)+"u\n"+ 
    Convert.ToString(percent)+"%\n"+Convert.ToString(change)+"Wh\n"+reactor,color); 
}
