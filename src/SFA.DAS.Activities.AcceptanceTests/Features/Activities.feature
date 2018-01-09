Feature: Activities
AS AN employer 
I WANT a list of activies 
SO THAT I can check it or use it for audit purpose


Scenario: Add Paye Scheme
When add_paye_scheme message get publish
Then I should have a PayeSchemeAdded Activity