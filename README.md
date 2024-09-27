# Tabs Navigation MvvmCross sample
## About
This repository contains a sample that extends default MvvmCross navigation with tabs for Android (since it already exists in iOS). The idea was to share navigation logic between Android and iOS and make the Android one match the iOS one.
## Research
The default iOS tabs navigation lets you have navigation back stacks for each tab separately. To achieve the same effect on Android I had 3 options in mind:
1. manually work with `FragmentManager`;
2. save back stacks separately and swap them since `FragmentManager` supports multiple back stacks;
3. extend already existing `ViewPager` navigation.

I decided to go with the 3rd one since it already fits better to the iOS solution where you navigate to all the tabs to create and show them. And the same goes for the `ViewPager` solution: you navigate to all the pages to display them. So the solution makes us to extend Android only and not touch iOS at all.
I seeked, found, and based on an already existing solution here: https://github.com/paulppn/MvvmCross/tree/feature/add_mvx_bottom_navigation_view (https://github.com/MvvmCross/MvvmCross/pull/3340)
## Implementation
### Source types
While the base implementation from the solution above is good, I had to extend it and fix some limitations and issues. The base implementation provides new 
- `MvxBottomNavigationViewPresentationAttribute`
- `MvxBottomNavigationView`
### New main types
but it does not provide multiple back stacks for each tab and works incorrectly on restoration. In my solution I provide
- `TabFragmentPresentationAttribute`
- `TabsViewPager`
- `TabsBottomNavigationView`
- `TabsFragment`

and it lets you have multiple back stacks for each tab. I also use https://github.com/anne-k/TabBarViewPagerAdapter + a similar code for `TabsBottomNavigationView` to fix the restoration issue on Android.

### Presentation
In total there are 3 new presentation attributes and a new presenter:
- `TabFragmentPresentationAttribute`
- `RootFragmentPresentationAttribute`
- `PushFragmentPresentationAttribute`
- `TabsNavigationViewPresenter`

`RootFragmentPresentationAttribute` guarantees to clear all the stacks and start a new one. `PushFragmentPresentationAttribute` will either navigate into current tab stack or will navigate into the root stack if tabs are not presented. And finally `TabFragmentPresentationAttribute` that is equivalent to `MvxTabPresentationAttribute` for iOS where you define your tab title and icon.
### New base `Fragment`s
To simplify the usage of the attributes it's better to just use new `BaseFragment`s:
- `BaseRootFragment`
- `BasePushFragment`
- `BaseTabFragment`
- `BaseTabPushFragment`
- `TabsFragment` (inherited from `BaseRootFragment`)

The new attributes are handled by the new `TabsNavigationViewPresenter`. The difference between base solution (`MvxBottomNavigationViewPresentationAttribute` + `MvxBottomNavigationView`) is that it now expects a trio of `TabFragmentPresentationAttribute`, `TabsViewPager`, `TabsBottomNavigationView`. The necessity of custom `TabsViewPager` is needed to use the new `TabsAdapter`: it fixes the restoration issue; it prevents `TabsViewPager` data to get refreshed until we navigated to all the tabs to prevent lags that got created by notifying on insertion of each new tab page. `TabsFragment` uses both `TabsViewPager` and `TabsBottomNavigationView` to handle all the logic. The only limitation is both `TabsFragment` and `TabsAdapter` **require the tabs count parameter** because `TabsAdapter` have to know when to refresh all the tab pages after the insertion of all of them. `TabsFragment` also handles Back system button clicks and closes the current tab back stack or switches to the default tab if there are no stacks left.

Also there is a special handling of `TabFragmentPresentationAttribute` in the new `TabsNavigationViewPresenter`: the tab `Fragment` that we want to display is not actually added to `TabsViewPager` directly. Instead there is special `WrapperTabFragment` that is added as a tab `Fragment`. But the actual our tab `Fragment` is added as a child into that `WrapperTabFragment`. It's needed to fix the issue with `Fragment`s transition animation where it does not apply animation to the root tab `Fragment`. So we have our root tab `WrapperTabFragment` with its simple `tab_page` layout that should not be touched at all.
## Usage
Using `MainLauncher` `Activity` and its VM as the first navigation VM is wide-spreaded but it's wrong since `MainLauncher` `Activity` will be launched anyways and navigation to the first VM will never actually happen in that scenario. So our `MainLauncher` `Activity` should be just a host for everything and contain `FragmentContainerView` to display all `Fragment`s including the `Fragment` that is bound to the first navigated VM.  
1. If you need to navigate first to `Fragment` with tabs then just inherit from `TabsFragment` and use your VM there.  
2. If it has to be a tabless `Fragment` then use `BaseRootFragment` (`TabsFragment` also inherits from `BaseRootFragment`).  
3. To display tabs inherit each tab `Fragment` from `BaseTabFragment` with corresponding VMs for each.  
4. To display a `Fragment` inside a tab and start a new stack there then use `BaseTabPushFragment`.  
5. If you want to display next `Fragment` outside of tabs or you don't have tabs at all then use `BasePushFragment` (`BaseTabPushFragment` works as `BasePushFragment` if there are no tabs).
## Extra
Since restoration process on Android is not that obvious and tricky, there are some bugs in MvvmCross that prevent using restoration correctly out of the box. One of them is first VM navigation.  
By default when the app starts it navigates to the first VM. But then when the app is destroyed by the system then it has to be restored afterward. But instead MvvmCross will force you to navigate to the first VM although the VM and the whole navigation stack are restored and no actions are needed: https://github.com/MvvmCross/MvvmCross/issues/4666. To fix the issue I check if `hint` is not `null` in custom `AppStart` since `hint` is `Bundle` and when it's not `null` then it's a restoration process and we don't need to navigate anywhere.  
Also since removal of `MvxSplashScreenActivity` there is no any first navigations at all on Android: https://github.com/MvvmCross/MvvmCross/pull/4846. To fix that I monitor the `MainLauncher` `Activity` lifecycle and `MvxSetup` lifecycle in `Application` to properly navigate to the first VM when everything is set up and ready.
## TODO
1. Remake tabs navigation with one single presentation attribute for Android and iOS where you pass an array of VM types and they will be added automatically by presenter.
2. Remake implementation for Android to use named back stacks instead of `ViewPager` (requires TODO #1 to be implementated first): https://developer.android.com/guide/fragments/fragmentmanager#multiple-back-stacks.
