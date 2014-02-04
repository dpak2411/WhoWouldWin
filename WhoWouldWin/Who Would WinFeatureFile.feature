Feature: WhoWOuldWIn
         This app pits two of your favorite characters against one another.Who do you think will win? 
             
                                     
 Background: 
  Given alteryx running at "http://gallery.alteryx.com/"
  And I am logged in using "deepak.manoharan@accionlabs.com" and "P@ssw0rd"
  
Scenario Outline: Who Would Win
When I run analog store analysis with ThisMatchUP "<ThisMatchUP>" AddChallengers "<AddChallengers>"
Then I see the WhoWouldWin result "<Result>"

Examples: 
| ThisMatchUP           | AddChallengers   | Result               |
| Captain America       | Hello            | Thanks for playing!  |