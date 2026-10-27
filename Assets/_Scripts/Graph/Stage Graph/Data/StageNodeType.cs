public enum StageNodeType
{
    None = -1,

    Start = 0,
    ReturnHome = 1,
    Dialogue = 2,
    Choice = 3,
    Sprite = 4,

    //연출
    CutScene = 100,
    Camera = 101,
    Audio = 102,

    //포커 관련
    Ante = 200,
    Draw = 201,
    Deal = 202,
    Roll = 203,
    Bet = 204,
    Open = 205,
    Showdown = 207
}
