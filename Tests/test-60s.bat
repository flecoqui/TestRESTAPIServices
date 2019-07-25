vegeta attack -duration=60s -rate 300 -targets=target-function.txt | vegeta report
vegeta attack -duration=60s -rate 1000 -targets=target-appservice.txt | vegeta report
vegeta attack -duration=60s -rate 8000 -targets=target-vm.txt | vegeta report
vegeta attack -duration=60s -rate 6000 -targets=target-aci.txt | vegeta report
vegeta attack -duration=60s -rate 10000 -targets=target-aks.txt | vegeta report
