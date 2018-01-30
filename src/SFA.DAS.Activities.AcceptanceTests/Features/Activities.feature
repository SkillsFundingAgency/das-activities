Feature: Activities
AS AN employer 
I WANT a list of activies 
SO THAT I can use it for auditing purposes

Scenario: Add Paye Scheme
Given a PayeSchemeAddedMessage message for Account A is published
When I get activities for Account A
Then I should have 1 PayeSchemeAdded activities

Scenario: Remove Paye Scheme
Given a PayeSchemeDeletedMessage message for Account A is published
When I get activities for Account A
Then I should have 1 PayeSchemeRemoved activities

Scenario: Agreement Signed
Given a AgreementSignedMessage message for Account A is published
When I get activities for Account A
Then I should have 1 AgreementSigned activities

Scenario: User Joined
Given a UserJoinedMessage message for Account A is published
When I get activities for Account A
Then I should have 1 UserJoined activities

Scenario: User Invited
Given a UserInvitedMessage message for Account A is published
When I get activities for Account A
Then I should have 1 UserInvited activities

Scenario: Legal Entity Removed
Given a LegalEntityRemovedMessage message for Account A is published
When I get activities for Account A
Then I should have 1 LegalEntityRemoved activities

Scenario: Legal Entity Added
Given a LegalEntityAddedMessage message for Account A is published
When I get activities for Account A
Then I should have 1 LegalEntityAdded activities

Scenario: Account Name Changed
Given a AccountNameChangedMessage message for Account A is published
When I get activities for Account A
Then I should have 1 AccountNameChanged activities

Scenario: Account Created
Given a AccountCreatedMessage message for Account A is published
When I get activities for Account A
Then I should have 1 AccountCreated activities

Scenario: Payment Created
Given a PaymentCreatedMessage message for Account A is published
When I get activities for Account A
Then I should have 1 PaymentCreated activities

Scenario: Get latest activities
Given a AccountCreatedMessage message for Account A and CreatedAt 0 days ago is published
Given a AccountCreatedMessage message for Account B and CreatedAt 0 days ago is published
Given a PayeSchemeAddedMessage message for Account A and CreatedAt 1 days ago is published
Given a PayeSchemeAddedMessage message for Account A and CreatedAt 0 days ago is published
Given a PayeSchemeDeletedMessage message for Account A and CreatedAt 0 days ago is published
Given a PayeSchemeDeletedMessage message for Account A and CreatedAt 0 days ago is published
When I get latest activities for Account A
Then I should have 1 AccountCreated activities for 1 aggregations
Then I should have 1 PayeSchemeAdded activities for 1 aggregations
Then I should have 2 PayeSchemeRemoved activities for 1 aggregations

Scenario: Get activities
Given a AccountCreatedMessage message for Account A and CreatedAt 0 days ago is published
Given a AccountCreatedMessage message for Account B and CreatedAt 0 days ago is published
Given a PayeSchemeAddedMessage message for Account A and CreatedAt 1 days ago is published
Given a PayeSchemeAddedMessage message for Account A and CreatedAt 0 days ago is published
Given a PayeSchemeDeletedMessage message for Account A and CreatedAt 0 days ago is published
Given a PayeSchemeDeletedMessage message for Account A and CreatedAt 0 days ago is published
When I get activities for Account A
Then I should have 1 AccountCreated activities
Then I should have 2 PayeSchemeAdded activities
Then I should have 2 PayeSchemeRemoved activities

Scenario: Get activities by Take
Given a PayeSchemeAddedMessage message for Account A and CreatedAt 1 days ago is published
Given a PayeSchemeAddedMessage message for Account A and CreatedAt 0 days ago is published
Given a PayeSchemeDeletedMessage message for Account A and CreatedAt 0 days ago is published
When I get activities for Account A and Take 2
Then I should have 1 PayeSchemeAdded activities
Then I should have 1 PayeSchemeRemoved activities

Scenario: Get activities by From and To
Given a PayeSchemeAddedMessage message for Account A and CreatedAt 3 days ago is published
Given a PayeSchemeAddedMessage message for Account A and CreatedAt 2 days ago is published
Given a PayeSchemeAddedMessage message for Account A and CreatedAt 1 days ago is published
Given a PayeSchemeAddedMessage message for Account A and CreatedAt 0 days ago is published
When I get activities for Account A and From 2 days ago and To 1 days ago
Then I should have 2 PayeSchemeAdded activities

Scenario: Get activities by PayeScheme
Given a PayeSchemeAddedMessage message for Account A and PayeScheme A is published
Given a PayeSchemeAddedMessage message for Account A and PayeScheme B is published
When I get activities for Account A and PayeScheme A
Then I should have 1 PayeSchemeAdded activities
