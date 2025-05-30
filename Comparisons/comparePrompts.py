from scipy.stats import kruskal


prompt_1 = [95.2, 94.6, 97.9, 98, 88.4, 98.2, 100, 100, 100, 100, 100]  # 11 values for ZeroShot
prompt_2 = [97.3, 93.4, 94.8, 94.7, 88.4, 98.2, 93.9, 100, 83.3, 100, 100]  # 11 values for FewShot
prompt_3 = [86.9, 100, 100, 100, 90.5, 98.2, 100, 100, 100, 100, 100]  # 11 values for CoT

# For Line Coverage
stat, p_value = kruskal(prompt_1, prompt_2, prompt_3)

print("line coverage")
print(f"Kruskal-Wallis H = {stat:.3f}, p = {p_value:.4f}")

prompt_1 = [89, 95, 94.5, 95.8, 83.8, 98, 87.5, 100, 100, 100, 100]  # 11 values for ZeroShot
prompt_2 = [95.4, 94.1, 94.5, 81.2, 83.8, 98, 85, 100, 88.4, 100, 100]  # 11 values for FewShot
prompt_3 = [78,7, 100, 100, 97.9, 87, 98, 87.5, 100, 100, 100]  # 11 values for CoT

# For Line Coverage
stat, p_value = kruskal(prompt_1, prompt_2, prompt_3)

print("branch coverage")
print(f"Kruskal-Wallis H = {stat:.3f}, p = {p_value:.4f}")

prompt_1 = [76.3, 91.5, 87.8, 91.9, 86, 90.7, 79, 87, 75.7, 88.5, 66.7]  # 11 values for ZeroShot
prompt_2 = [83.9, 91.1, 87.0, 89.3, 85.0, 86.9, 66.3, 87.0, 16.2, 100, 75.9]  # 11 values for FewShot
prompt_3 = [78.6, 87.4, 81.8, 98.4, 82.7, 93.5, 84.0, 91.3, 74.3, 84.6, 66.7]  # 11 values for CoT

# For Line Coverage
stat, p_value = kruskal(prompt_1, prompt_2, prompt_3)

print("mutation score")
print(f"Kruskal-Wallis H = {stat:.3f}, p = {p_value:.4f}")
