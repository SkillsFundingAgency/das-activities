Feature: Activities
AS AN employer 
I WANT a list of activies 
SO THAT I can check it or use it for audit purpose


Scenario: Add Paye Scheme
When PayeSchemeAddedMessage message get publish
Then I should have a PayeSchemeAdded Activity

Scenario: Remove Paye Scheme
When PayeSchemeDeletedMessage message get publish
Then I should have a PayeSchemeRemoved Activity