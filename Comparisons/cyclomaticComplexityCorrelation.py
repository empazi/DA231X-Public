import pandas as pd
from scipy.stats import spearmanr
import matplotlib.pyplot as plt

# ZeroShot data
dataZeroShot = {
    'Class': ['Class1', 'Class2', 'Class3', 'Class4', 'Class5', 'Class6', 'Class7', 'Class8', 'Class9', 'Class10', 'Class11'],
    'CyclomaticComplexity': [137, 101, 100, 74, 45, 42, 24, 24, 20, 18, 9],
    'LineCoverage': [95.2, 94.6, 97.9, 98, 88.4, 98.2, 100, 100, 100, 100, 100],
    'BranchCoverage': [89, 95, 94.5, 95.8, 83.8, 98, 87.5, 100, 100, 100, 100],
    'MutationScore': [76.3, 91.5, 87.8, 91.9, 86, 90.7, 79, 87, 75.7, 88.5, 66.7]
}

# FewShot data
dataFewShot = {
    'Class': ['Class1', 'Class2', 'Class3', 'Class4', 'Class5', 'Class6', 'Class7', 'Class8', 'Class9', 'Class10', 'Class11'],
    'CyclomaticComplexity': [137, 101, 100, 74, 45, 42, 24, 24, 20, 18, 9],
    'LineCoverage': [97.3, 93.4, 94.8, 94.7, 88.4, 98.2, 93.9, 100, 83.3, 100, 100],
    'BranchCoverage': [95.4, 94.1, 94.5, 81.2, 83.8, 98, 85, 100, 88.4, 100, 100],
    'MutationScore': [83.9, 91.1, 87.0, 89.3, 85.0, 86.9, 66.3, 87.0, 16.2, 100, 75.9]
}

# CoT data
dataCoT = {
    'Class': ['Class1', 'Class2', 'Class3', 'Class4', 'Class5', 'Class6', 'Class7', 'Class8', 'Class9', 'Class10', 'Class11'],
    'CyclomaticComplexity': [137, 101, 100, 74, 45, 42, 24, 24, 20, 18, 9],
    'LineCoverage': [86.9, 100, 100, 100, 90.5, 98.2, 100, 100, 100, 100, 100],
    'BranchCoverage': [78,7, 100, 100, 97.9, 87, 98, 87.5, 100, 100, 100],
    'MutationScore': [78.6, 87.4, 81.8, 98.4, 82.7, 93.5, 84.0, 91.3, 74.3, 84.6, 66.7]
}

dfZeroShot = pd.DataFrame(dataZeroShot)
dfFewShot = pd.DataFrame(dataFewShot)
dfCoT = pd.DataFrame(dataCoT)

# Calculate Spearman's correlation
for metric in ['LineCoverage', 'BranchCoverage', 'MutationScore']:
    coef, p = spearmanr(dfZeroShot['CyclomaticComplexity'], dfZeroShot[metric])
    print("ZeroShot")
    print(f"Spearman correlation between Cyclomatic Complexity and {metric}:")
    print(f"Coefficient = {coef:.3f}, p-value = {p:.3f}\n")

# Calculate Spearman's correlation
for metric in ['LineCoverage', 'BranchCoverage', 'MutationScore']:
    coef, p = spearmanr(dfFewShot['CyclomaticComplexity'], dfFewShot[metric])
    print("FewShot")
    print(f"Spearman correlation between Cyclomatic Complexity and {metric}:")
    print(f"Coefficient = {coef:.3f}, p-value = {p:.3f}\n")

# Calculate Spearman's correlation
for metric in ['LineCoverage', 'BranchCoverage', 'MutationScore']:
    coef, p = spearmanr(dfCoT['CyclomaticComplexity'], dfCoT[metric])
    print("CoT")
    print(f"Spearman correlation between Cyclomatic Complexity and {metric}:")
    print(f"Coefficient = {coef:.3f}, p-value = {p:.3f}\n")



metrics = ['LineCoverage', 'BranchCoverage', 'MutationScore']

for metric in metrics:
    plt.figure()
    plt.scatter(dfZeroShot['CyclomaticComplexity'], dfZeroShot[metric])
    plt.title(f'Cyclomatic Complexity vs {metric}')
    plt.xlabel('Cyclomatic Complexity')
    plt.ylabel(metric)
    plt.grid(True)
    plt.savefig(f'{metric}_vs_CC.png', dpi=300, bbox_inches='tight')
    plt.close()
