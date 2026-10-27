public class SettingData
{
    public float MasterVolume = 1f;
    public float BGMVolume = 1f;
    public float SFXVolume = 1f;
    public float VoiceVolume = 1f;

    public float TypingSpeed = TypingSpeedConfigs.NORMAL;

    public int resolutionIndex = 0;

    public SettingData()
    {
        this.MasterVolume = 1f;
        this.BGMVolume = 1f;
        this.SFXVolume = 1f;
        this.VoiceVolume = 1f;

        this.TypingSpeed = TypingSpeedConfigs.NORMAL;
    }
}