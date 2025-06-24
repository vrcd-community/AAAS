# OpenGroupPage

## OpenGroupPage Inspector Parameters
* `String` **Group Id** - The group id of the group you want to open. You can find the group id by opening the group on the website and copying the ID in the address bar of your browser. For example: grp_a4f791af-a167-4c91-b849-2e37e37f509a
* `Bool` **Open To Store Page** - If true, will open the store page for the group instead of the group info page.

## OpenListing Inspector Parameters
* `String` **Listing Id** - The listing id of the listing you want to open.


## Description
A simple prefab to open a group page. Change the group id and change the `Open To Store Page` toggle to your preferred use case.
Also includes an example to open directly to a listing.

### Example UI includes the following buttons
* Open Group: Opens the group's page
* Open Listing: Opens the listing's page

### Prefabs included
* OpenGroupPagePrefab: An example with button to open to a group page which also includes text for description.
* OpenGroupPageSimplePrefab: An example with just the button to open to a group page.
* OpenListingPrefab: An example with button to open to a listing page which also includes text for description.
* OpenListingSimplePrefab: An example with just the button to open to a listing page.

## Example Use Cases
* Having a button for players to open your group page so they can subscribe to your group.
* Having a button for players to open a listing page so they can buy a product.

---
## How to Use This Example
1. Set the id of the group that owns the product in the `Group Id` field on the OpenGroupPage prefab and/or the id of the listing in the `Listing Id` field on the OpenListing prefab.
2. Toggle in the "OpenToStorePage" toggle on the OpenGroupPage prefab if you want to open to store page directly, otherwise leave unchecked.
3. Run Build & Test!