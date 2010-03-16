These are integration tests, so we use the real classes of CCNet as much as possible.
Mocks should not be used as we want to test the real classes, mocks like an e-mail server are ok
The idea is to eliminate regression : re-introduce bugs that were fixed once.

