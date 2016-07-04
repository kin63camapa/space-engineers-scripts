
string controlVentName = "Control Vent";
string displaysPrefix = "O2 Info";
string alarmsPrefix = "Alarm";
string lampsPrefix = "Light O2";
//must be a pair!
string gatewaysPrefix = "GW ";
string gatewaysInternalPrefix = "INT";
string gatewaysExternalPrefix = "EXT";
//must be a pair!
string exceptDoorsPrefix = "External door";
string doorLampsPrefix = "Door Light";
string controlTankName = "Баллон с кислородом";
string gensCommonName = "Генератор кислорода";


string tpstate = "Generators fail!";

void alarm()
{
    List<IMyTerminalBlock> speakers = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(speakers);
    for(int i = 0; i < speakers.Count; i++)
    {
        IMySoundBlock speaker = (IMySoundBlock) speakers[i];
        if(speaker.CustomName.IndexOf(alarmsPrefix) == 0)
        {
            speaker.ApplyAction("PlaySound");
            //speaker.ApplyAction("IncreaseLoopableSlider");
        }
    }
}

void setDL(bool on) 
{ 
    List<IMyTerminalBlock> lights = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(lights); 
    for(int i = 0; i < lights.Count; i++) 
    { 
        IMyLightingBlock light = (IMyLightingBlock) lights[i]; 
        if(light.CustomName.IndexOf(doorLampsPrefix) == 0) 
        { 
            if(on)
            {
                light.ApplyAction("OnOff_On");
                light.SetValue("Color",Color.Green);
            }else{
                //light.ApplyAction("OnOff_Off");
                light.SetValue("Color",Color.Red);
            }
        } 
    } 
}

void closeDoors(bool most,bool except)
{
    List<IMyTerminalBlock> doors = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.GetBlocksOfType<IMyDoor>(doors); 
    for(int i = 0; i < doors.Count; i++) 
    {
        if(doors[i].CustomName.IndexOf(gatewaysPrefix+gatewaysExternalPrefix) == 0)
        {
            IMyDoor door = (IMyDoor) doors[i];
            if (door.Open)
            {
                IMyDoor gw2 = (IMyDoor) GridTerminalSystem.GetBlockWithName(
                    gatewaysPrefix+gatewaysInternalPrefix+door.CustomName.Substring(
                    gatewaysPrefix.Length+gatewaysExternalPrefix.Length));
                if (gw2.Open)
                {
                    alarm();
                    gw2.ApplyAction("Open_Off");
                }
            }
            continue;
        }
                if(doors[i].CustomName.IndexOf(gatewaysPrefix+gatewaysInternalPrefix) == 0)
        {
            IMyDoor door = (IMyDoor) doors[i];
            if (door.Open)
            {
                IMyDoor gw2 = (IMyDoor) GridTerminalSystem.GetBlockWithName(
                    gatewaysPrefix+gatewaysExternalPrefix+door.CustomName.Substring(
                    gatewaysPrefix.Length+gatewaysInternalPrefix.Length));
                if (gw2.Open)
                {
                    alarm();
                    gw2.ApplyAction("Open_Off");
                }
            }
            continue;
        }
        if (doors[i].CustomName.IndexOf(exceptDoorsPrefix) == 0)
        {
            if (except)
            {
                doors[i].ApplyAction("Open_Off");
            } else continue;
        }
        if (most) doors[i].ApplyAction("Open_Off"); 
    } 
}

void setLightColor(Color color)
{
    List<IMyTerminalBlock> ligts = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(ligts);
    for(int i = 0; i < ligts.Count; i++)
    {
        IMyLightingBlock ligt = (IMyLightingBlock) ligts[i];
        if(ligt.CustomName.IndexOf(lampsPrefix) == 0)
        {
            ligt.SetValue("Color", color);
        }
    }
}

void swithGens(bool on)
{
    List<IMyTerminalBlock> ogs = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyOxygenGenerator>(ogs);
    for(int i = 0; i < ogs.Count; i++)
    {
        IMyOxygenGenerator og = (IMyOxygenGenerator) ogs[i];
        if(og.CustomName.IndexOf(gensCommonName) == 0)
        {
            if (on)
            {
                og.ApplyAction("OnOff_On");
                tpstate = "\r\n Generators ON!";
                
            }else{
                og.ApplyAction("OnOff_Off");
                tpstate = "\r\n Generators OFF!";
            }
        }
    }
}

void stopAlarm()
{
    List<IMyTerminalBlock> speakers = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(speakers);
    for(int i = 0; i < speakers.Count; i++)
    {
        IMySoundBlock speaker = (IMySoundBlock) speakers[i];
        if(speaker.CustomName.IndexOf(alarmsPrefix) == 0)
        {
            speaker.ApplyAction("StopSound");
        }
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

void Main()
{
    IMyAirVent vent = (IMyAirVent) GridTerminalSystem.GetBlockWithName(controlVentName);
    IMyOxygenTank tank = (IMyOxygenTank) GridTerminalSystem.GetBlockWithName(controlTankName);
    string pressure  = "0.00%";
    int pr =0;
    pressure = tank.DetailedInfo.Split(new string[] { "\n" },StringSplitOptions.None)[2].Substring(8);
    int tp = Convert.ToInt32(pressure.Substring(0,pressure.IndexOf(".")));
    pressure = "0.00%";
    Color color = Color.Red;
    if(vent.DetailedInfo.IndexOf("Not pressurized") == -1)
    {
        pressure = vent.DetailedInfo.Split(new string[] { "\n" }, StringSplitOptions.None)[2].Substring(15);
        pr = Convert.ToInt32(pressure.Substring(0,pressure.IndexOf(".")));
    }
    if(pr > 60)
    {
        setDL(false);
        color = Color.Green;
        //stopAlarm();
        setLightColor(Color.White);
    }
    else
    {
        alarm();
        setLightColor(Color.Red);
        if(pr < 10)
        {
            color = Color.Red;
            setDL(true);
        }else
        {
             setDL(false);
             color = Color.Orange;
        }
    }
    closeDoors(pr < 60,pr > 10);
    swithGens((pr+tp) < 99);
    setScreensText(" Oxygen: "+pressure+"\r\n Tanks: "+
        tank.DetailedInfo.Split(new string[] { "\n" },StringSplitOptions.None)[2].Substring(8)+tpstate,color);
    return;
}
