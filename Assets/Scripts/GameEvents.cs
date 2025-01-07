using System;

public static class GameEvents
{
   public static Action<int, int> OnScoreAddedByType; // Pass points and type
   public static Action<int> OnMoveDecrease; // Triggered whenever moves have decreased
}
