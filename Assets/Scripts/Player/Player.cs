using System.Collections.Generic;
using UnityEngine;
using Logger = LNAR.Logger;
/**
    This Enum will be used to keep track of which player
    the Piece belongs to as well has whose turn it is.
*/
public enum PlayerEnum { Player1, Player2, NotSet }

public struct MatchRecord {
  public Player Player;
  public Player Opponent;
  public Player Winner;

  public MatchRecord(Player player, Player opponent, Player winner) {
    this.Player = player;
    this.Opponent = opponent;
    this.Winner = winner;
  }
  /** Used to Write to the textfile
   * The string is ordered (Player, Opponent, Winner)
   */
  public override string ToString() {
    return $"({Player.Name}, {Opponent.Name}, {Winner.Name})";
  }
}

/**
 * Holds all useful information about a given player
 * Players can either be player1 or player2
 * based on PlayerEnum, but can also have unique
 * attributes for example Piece colour, name, stats
 */
public class Player {
  public PlayerEnum PlayerNum = PlayerEnum.NotSet;

  // Records for the db, this should be loaded in when the player is selected
  private int _wins = -1;
  public int Wins {
    get => _wins;
  private
    set { _wins = value; }
  }
  private int _losses = -1;
  public int Losses {
    get => _losses;
  private
    set { _losses = value; }
  }
  private string _name = null;
  public string Name {
    get => _name;
  private
    set { _name = value; }
  }

  // This should only be interacted with using
  // Player::AddMatchHistory(MatchRecord )
  // Player::GetMatchHistory()
  // Maybe I should use a property for this but it seems clear to only use properities on only
  // methods
  private Queue<MatchRecord> _matchHistory;

  public PlayerEnum GetPlayerNum() {
    return PlayerNum;
  }
  public Player(PlayerEnum pNum) {
    this.PlayerNum = pNum;
  }

  // This will load the player in from the text file
  public Player(string name, PlayerEnum pNum) : this(pNum) {
    Name = name;
  }

  /** Gets the Piece Color in the future this will be
   *  based of profiles colour but for now it is just hard coded
   */
  public Color GetPlayerColour() {
    if (PlayerNum == PlayerEnum.Player1) {
      return Color.white;
    } else if (PlayerNum == PlayerEnum.Player2) {
      return Color.red;
    } else {
      return Color.green;
    }
  }

  /**
   * Adds a match to the current history if there is more than 5 it pops out the oldest one
   */
  public bool AddMatchHistory(MatchRecord record) {
    bool matchAdded = true;

    // error checking
    if (record.Player == null || record.Opponent == null || record.Winner == null) {
      Logger.Info($"Invalid match record");
      matchAdded = false;
    } else if (record.Player.Name == null || record.Player.Wins == -1 ||
               record.Player.Losses == -1 || record.Opponent.Name == null ||
               record.Opponent.Wins == -1 || record.Opponent.Losses == -1) {
      Logger.Info($"Player objects aren't fully init'd");
      matchAdded = false;
    } else {  // No errors
      // only edit my match history other players will worry about their match history
      if (record.Winner == this) {
        this.Wins++;
      } else {
        this.Losses++;
      }

      _matchHistory.Enqueue(record);
      // remove old match if it contains more than 5 records
      if (_matchHistory.Count > 5)
        _matchHistory.Dequeue();
    }
    return matchAdded;
  }

  public override string ToString() {
    string playerString = $"{Name},{Wins},{Losses}";
    foreach (var match in _matchHistory) {
      playerString += $",{match.ToString()}";
    }
    return playerString;
  }

  // Felt like using one of the patterns we learned in class so here is a factory
  public static Player CreateNewPlayer(string name, PlayerEnum pNum) {
    Player player = new Player(name, pNum);
    player._losses = 0;
    player._wins = 0;
    player._matchHistory = new Queue<MatchRecord>();
    return player;
    // TODO write this to db
  }

  // Only used for unit tests use CreateNewPlayer() for production.
  public static Player CreateNewPlayerUnitTest(string name, PlayerEnum pNum, int wins, int losses) {
    Player player = new Player(name, pNum);
    player._wins = wins;
    player._losses = losses;
    // NOTE: this should not be written to any text file this is to create mock objects for unit
    // testing.
    return player;
  }

  // Only used for unit tests
  public void SetMatchHistoryUnitTest(Queue<MatchRecord> history) {
    this._matchHistory = history;
  }
}