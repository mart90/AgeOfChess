# AgeOfChess

**Piece movement/placement**
- The pieces move like in Chess with one exception: Pawns can move any direction in a straight line, and capture any direction diagonally.
- Rocks block movement along files and diagonals and nothing can stand on them.
- Trees block movement along files and diagonals but pieces can stand on them.
- Mines (rocks with gold in them) work the same as trees.
- New pieces can only be placed around your king, with the exception of pawns which can be placed around any major piece.


**Two additional win conditions compared to regular Chess:**
- If a player gets to 150 gold, they win the game. If both players get there on the same turn, the highest gold count wins. If it's equal, the first player to gain a gold lead wins.
- Stalemate wins the game instead of drawing it.


Treasures give 20 gold.
Mines  give 3 gold each turn you stand on them with any piece (including on the opponent's turn).


**Bidding explanation**

Due to the first move advantage, black starts with 10 gold by default. However, the first move advantage differs per map spawn, so 10 isn't always the right number. To balance this, with bidding enabled players get the opportunity to bid for white. The highest bidder gets white, and the opponent gets black with gold equal to white's bid. 

Example:
Player A thinks the first move is worth 8 gold and bids 8.
Player B thinks it's worth more and bids 10.
Result: Player B gets white. Player A gets black, starting with 10 gold. 


![image](https://user-images.githubusercontent.com/30334336/182841854-4b3d3852-dc68-429e-978f-ac722fa288ed.png)
