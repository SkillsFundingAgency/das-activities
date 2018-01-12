Feature: Activities
AS AN employer 
I WANT a list of activies 
SO THAT I can check it or use it for audit purpose


Scenario: Add Paye Scheme
When PayeSchemeAddedMessage message get publish
Then I should have a PayeSchemeAdded Activity