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

Scenario: Agreement Signed
When AgreementSignedMessage message get publish
Then I should have a AgreementSigned Activity

Scenario: User Joined
When UserJoinedMessage message get publish
Then I should have a UserJoined Activity

Scenario: User Invited
When UserInvitedMessage message get publish
Then I should have a UserInvited Activity

Scenario: Legal Entity Removed
When LegalEntityRemovedMessage message get publish
Then I should have a LegalEntityRemoved Activity

Scenario: Legal Entity Added
When LegalEntityAddedMessage message get publish
Then I should have a LegalEntityAdded Activity

Scenario: Account Name changed
When AccountNameChangedMessage message get publish
Then I should have a AccountNameChanged Activity

Scenario: Account Created
When AccountCreatedMessage message get publish
Then I should have a AccountCreated Activity
