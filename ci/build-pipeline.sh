#!/bin/bash
set -e

cp pipeline-resources.yml merged-pipeline.yml
echo -e "\n" >> merged-pipeline.yml
cat pipeline-resources-template.yml | sed -e "s/\[ENV\]/acceptance/" >> merged-pipeline.yml
echo -e "\n">> merged-pipeline.yml
cat pipeline-resources-template.yml | sed -e "s/\[ENV\]/int/" >> merged-pipeline.yml
echo -e "\n\njobs:\n" >> merged-pipeline.yml
cat pipeline-jobs.yml >> merged-pipeline.yml
echo -e "\n">> merged-pipeline.yml
cat pipeline-jobs-template.yml | sed -e "s/\[ENV\]/acceptance/" >> merged-pipeline.yml
echo -e "\n">> merged-pipeline.yml
cat pipeline-jobs-template.yml | sed -e "s/\[ENV\]/int/" >> merged-pipeline.yml

echo "merged pipeline file created successfully"
echo "run 'fly -t [target] sp -p GoFNOL -c merged-pipeline.yml' to update pipeline"